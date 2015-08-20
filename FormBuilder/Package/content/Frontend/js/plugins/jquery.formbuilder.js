(function($, window, document, undefined) {
    var getRendererSettings = function(form) {
        var renderer = form.data('renderer');

        if (typeof renderer === 'string') {
            $.ajax({
                type: 'GET',
                url: '/formbuilder/renderer/settings?type=' + renderer,
                dataType: 'json',
                async: false,
                success: function(data) {
                    renderer = $.parseJSON(data);

                    form.data('renderer', renderer);
                }
            });
        }

        return renderer;
    };

    var getValidationSummary = function(form, errors) {
        var data = {
            renderer: form.attr('data-renderer'),
            errors: errors
        };

        var summary;

        $.ajax({
            type: 'POST',
            url: '/formbuilder/renderer/validationsummary',
            data: JSON.stringify(data),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: false,

            success: function(data) {
                summary = $(data);
            },

            failure: function() {
                var rendererSettings = getRendererSettings(form);
                var errorDiv = $('<div />').addClass(rendererSettings.ValidationSummaryClass);
                var errorList = $('<ul>').appendTo(errorDiv);

                $.each(errors, function() {
                    var message = this.Message || this.message;

                    errorList.append('<li>' + message + '</li>');
                });

                summary = errorDiv;
            }
        });

        return summary;
    };

    var validation = function(form, options) {
        formbuilder.clearErrors(form);
        formbuilder.validate(form, form.serializeArray(), options);
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

    $(document).ready(function() {
        var forms = $('form[class^="form formbuilder-"]');

        forms.on('change', ':input', function() {
            var form = $(this).parents('form');

            formbuilder.dependecyFunction(form);
        });

        var btnSelector = 'input[type=submit]';

        if (window.Ladda) {
            $.each(forms, function() {
                var submitButtons = $(btnSelector, this);

                $.each(submitButtons, function() {
                    var btn = $(this);
                    var val = btn.val();
                    var html = '<button class="ladda-button btn btn-primary" data-style="expand-right" type="submit" name="SubmitForm" value="' + val + '">' + val + '</button>';

                    btn.replaceWith(html);
                });
            });

            btnSelector = 'button[type=submit]';
        }

        forms.on('click', btnSelector, function() {
            var btn = $(this);
            var form = btn.parents('form');
            var submitButtons = $(btnSelector, form);

            submitButtons.removeAttr('clicked');
            btn.attr('clicked', true);
        });

        forms.on('submit', function(e) {
            var form = $(this);
            var clickedButton = $(btnSelector + '[clicked=true]', form);

            clickedButton.attr('disabled', true);

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

            validation(form, {
                onError: function() {
                    clickedButton.attr('disabled', false);
                    hiddenField.remove();

                    if (l !== undefined) {
                        l.stop();
                    }

                    window.scrollTo(0, 0);

                    e.preventDefault();
                }
            });
        });
    });

    window.formbuilder = window.formbuilder || {};

    formbuilder.validate = function(form, formSerialized, options) {
        options = $.extend({}, options);

        var validateEvent = $.Event('formbuilder.validate');
        validateEvent.options = options;

        form.trigger(validateEvent);

        if (validateEvent.isDefaultPrevented()) {
            return;
        }

        var rendererSettings = getRendererSettings(form);

        $.ajax({
            type: 'POST',
            url: '/formbuilder/validation',
            data: formSerialized,
            dataType: 'json',
            async: false,

            success: function(data) {
                var validatedEvent = $.Event('formbuilder.validated');
                validatedEvent.errors = data;

                if (validatedEvent.errors.length > 0) {
                    var validationSummary = getValidationSummary(form, validatedEvent.errors);

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
                    validationSummaryShowingEvent.summary = validationSummary;

                    form.trigger(validationSummaryShowingEvent);

                    if (!validationSummaryShowingEvent.isDefaultPrevented()) {
                        form.prepend(validationSummary);
                    }

                    if (typeof options.onError === 'function') {
                        options.onError();
                    }
                }

                form.trigger(validatedEvent);
            }
        });
    };

    formbuilder.clearErrors = function(form) {
        var rendererSettings = getRendererSettings(form);

        $('.' + rendererSettings.ParentGroupClass + '-group', form).removeClass(rendererSettings.ErrorClass);
        $('.' + rendererSettings.ValidationSummaryClass, form).remove();

        form.data('error', false);
    };

    formbuilder.dependecyFunction = function(form) {
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
})(jQuery, window, document);