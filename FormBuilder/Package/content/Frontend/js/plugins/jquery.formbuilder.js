(function($, window, document, undefined) {
    var formbuilder = {};
    var selector = 'form[class^="form formbuilder-"]';

    var getRendererSettings = function(form) {
        return form.data('renderer').Settings;
    };

    var getValidationSummary = function(form, errors) {
        var dfd = $.Deferred();

        var data = {
            renderer: form.data('renderer').Type,
            errors: errors
        };

        $.ajax({
            type: 'POST',
            url: '/formbuilder/renderer/validationsummary',
            data: JSON.stringify(data),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',

            success: function(data) {
                var el = $(data);

                dfd.resolve(el);
            },

            failure: function() {
                var rendererSettings = getRendererSettings(form);

                var errorDiv = $('<div />').addClass(rendererSettings.ValidationSummaryClass);
                var errorList = $('<ul>').appendTo(errorDiv);

                $.each(errors, function() {
                    var message = this.Message || this.message;

                    errorList.append('<li>' + message + '</li>');
                });

                dfd.resolve(errorDiv);
            }
        });

        return dfd.promise();
    };

    var validation = function(form) {
        var dfd = $.Deferred();

        form.formbuilder('clearErrors');

        form.formbuilder('validate', form.serializeArray()).fail(function() {
            dfd.reject();
        }).done(function() {
            dfd.resolve();
        });

        return dfd.promise();
    };

    var getFormFieldValue = function(form, fieldName) {
        var rendererSettings = getRendererSettings(form);

        var field = $('[name="' + fieldName + '"]');

        if (field.is(':radio')) {
            field = field.filter(':checked');
        }

        var element = field.eq(0);
        var hidden = false;

        while (element.length > 0) {
            if (element.hasClass(rendererSettings.ParentGroupClass + '-group')) {
                hidden = hidden || !element.is(':visible');
            }

            element = element.parent();
        }

        if (hidden) {
            return null;
        }

        if (field.is(':checkbox')) {
            if (field.length > 1) {
                return $.map(field.filter(':checked'), function(itm) {
                    return $(itm).val();
                });
            } else {
                return field.is(':checked').toString();
            }
        }

        return field.val();
    };

    var escapeRegExp = function(str) {
        return str.toString().replace(/[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g, '\\$&');
    }

    var isMatch = function(form, field, itm) {
        var fieldValue = getFormFieldValue(form, field);
        if (fieldValue) {
            var regexp = new RegExp('^' + escapeRegExp(itm) + '$', 'i');

            if (fieldValue instanceof Array) {
                var match = false;

                $.each(fieldValue, function() {
                    match = match || regexp.test(this);
                });

                return match;
            }

            return regexp.test(fieldValue);
        }

        return false;
    };

    var showFunction = function(form, json) {
        var show = false;

        $.each(json, function() {
            var field = this.field;
            var fieldValid = false;

            $.each(this.value, function() {
                fieldValid = fieldValid || isMatch(form, field, this);
            });

            if (!fieldValid) {
                show = false;

                return false;
            }

            show = fieldValid;

            return true;
        });

        return show;
    };

    var _init = function(form) {
        form.on('change', ':input', function() {
            var form = $(this).parents('form');

            form.formbuilder('dependecyFunction');
        });

        var btnSelector = 'input[type=submit]';

        if (window.Ladda) {
            var submitButtons = $(btnSelector, form);

            $.each(submitButtons, function() {
                var btn = $(this);
                var val = btn.val();

                var replacement = $('<button />', {
                    'class': btn.attr('class') + ' ladda-button',
                    'data-style': 'expand-right',
                    'type': 'submit',
                    'name': btn.attr('name'),
                    'value': val
                }).html(val);

                btn.replaceWith(replacement);
            });

            btnSelector = 'button[type=submit]';
        }

        form.on('click', btnSelector, function() {
            var btn = $(this);
            var form = btn.parents('form');
            var submitButtons = $(btnSelector, form);

            submitButtons.removeData('clicked');
            btn.data('clicked', true);
        });

        form.on('submit', function(e) {
            if (form.data('validated') === true) {
                return;
            }

            var clickedButton = $(btnSelector, form).filter(function() {
                return $(this).data('clicked') === true;
            });

            clickedButton.attr('disabled', 'disabled');

            var clickedButtonName = clickedButton.attr('name');
            var clickedButtonValue = clickedButton.val();

            var hiddenField = $('<input />').attr({
                type: 'hidden',
                name: clickedButtonName,
                value: clickedButtonValue
            }).appendTo(form);

            var l = undefined;

            if (window.Ladda) {
                setTimeout(function() {
                    if (clickedButton.is(':disabled')) {
                        l = window.Ladda.create(clickedButton[0]);

                        l.start();
                    }
                }, 500);
            }

            e.preventDefault();

            validation(form).fail(function() {
                clickedButton.removeAttr('disabled');
                hiddenField.remove();

                if (l !== undefined) {
                    l.stop();
                }

                var el = form.get(0);
                var rect = el.getBoundingClientRect();

                if (rect <= 0) {
                    el.scrollIntoView();
                }
            }).done(function() {
                form.data('validated', true);

                form.submit();
            });
        });
    };

    var _validate = function(form, formSerialized) {
        var dfd = $.Deferred();

        var validateEvent = $.Event('formbuilder.validate');

        form.trigger(validateEvent);

        if (validateEvent.isDefaultPrevented()) {
            dfd.resolve();
        } else {
            var rendererSettings = getRendererSettings(form);

            $.ajax({
                type: 'POST',
                url: '/formbuilder/validation',
                data: formSerialized,
                dataType: 'json',

                success: function(data) {
                    var validatedEvent = $.Event('formbuilder.validated');
                    validatedEvent.errors = data;

                    form.trigger(validatedEvent);

                    if (validatedEvent.errors.length > 0) {
                        form.data('error', true);

                        $.each(validatedEvent.errors, function() {
                            var fields = this.AffectedFields || this.affectedFields;

                            $.each(fields, function() {
                                var el = $('[name="' + this + '"]', form);

                                el.parents('.' + rendererSettings.ParentGroupClass + '-group').addClass(rendererSettings.ErrorClass);
                            });
                        });

                        var validationSummaryShowingEvent = $.Event('formbuilder.validationsummary.showing');
                        validationSummaryShowingEvent.errors = validatedEvent.errors;

                        getValidationSummary(form, validatedEvent.errors).then(function(summary) {
                            validationSummaryShowingEvent.summary = summary;

                            form.trigger(validationSummaryShowingEvent);

                            if (!validationSummaryShowingEvent.isDefaultPrevented()) {
                                form.prepend(summary);
                            }

                            dfd.reject();
                        });
                    } else {
                        dfd.resolve();
                    }
                }
            });
        }

        return dfd.promise();
    };

    var _clearErrors = function(form) {
        var rendererSettings = getRendererSettings(form);

        $('.' + rendererSettings.ParentGroupClass + '-group', form).removeClass(rendererSettings.ErrorClass);
        $('.' + rendererSettings.ValidationSummaryClass, form).remove();

        form.data('error', false);
    };

    var _dependecyFunction = function(form) {
        // We loop this function twice so controls that are depenent on eachother on the same "level" gets 
        // shown correctly. If we don't do this, we risk a control not showing up because its depentent on 
        // the next control, which is hidden during the first run.
        for (var i = 0; i < 2; i++) {
            $('[data-dependency]', form).each(function() {
                var itm = $(this);
                var json = itm.data('dependency');
                var show = showFunction(form, json);

                if (!show) {
                    itm.addClass('hide');
                } else {
                    itm.removeClass('hide');
                }
            });
        }
    };

    formbuilder.validate = _validate;
    formbuilder.clearErrors = _clearErrors;
    formbuilder.dependecyFunction = _dependecyFunction;

    formbuilder.initializers = [];

    formbuilder.initializers.push({
        selector: selector,
        initializer: _init
    });

    window.formbuilder = formbuilder;

    $.fn.formbuilder = function(options) {
        if ($.type(options) === 'string') {
            var scope = formbuilder;

            var cmd = options;
            if (cmd.indexOf('.') > -1) {
                var split = cmd.split('.');
                var module = split[0];

                cmd = split[1];
                scope = scope[module];
            }

            var cmdFn = scope[cmd];

            if ($.isFunction(cmdFn)) {
                var args = $.makeArray(arguments);

                args.shift();
                args.unshift(this);

                return cmdFn.apply(this, args);
            }
        } else {
            return this.each(function() {
                var form = $(this);

                $.each(formbuilder.initializers, function() {
                    var initializer = this;

                    if (form.is(initializer.selector)) {
                        initializer.initializer(form);
                    }

                    return form;
                });
            });
        }
    };

    $(document).ready(function() {
        var forms = $(selector);

        forms.formbuilder();
    });
})(jQuery, window, document);