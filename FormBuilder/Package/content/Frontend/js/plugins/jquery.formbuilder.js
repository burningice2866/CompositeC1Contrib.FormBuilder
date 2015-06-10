(function($, window, document, undefined) {
    window.formbuilder = window.formbuilder || {};

    formbuilder.validate = function(form, formSerialized, callback) {
        var rendererSettings = $(form).data('renderersettings');

        $.ajax({
            type: 'POST',
            url: '/formbuilder/validation',
            data: formSerialized,
            dataType: 'json',
            async: false,
            success: function(data) {
                var errors = data;
                if (errors.length > 0) {
                    form.data('error', true);

                    var errorDiv = '<div class="' + rendererSettings.ErrorNotificationClass + '"><p>Du mangler at udfylde nogle felter</p><ul>';

                    $.each(errors, function(i, itm) {
                        var fields = itm.AffectedFields || itm.affectedFields;
                        var message = itm.Message || itm.message;

                        $.each(fields, function(i, field) {
                            var el = $('[name="' + field + '"]', form);

                            el.parents('.' + rendererSettings.ParentGroupClass + '-group').addClass(rendererSettings.ErrorClass);
                        });

                        errorDiv += '<li>' + message + '</li>';
                    });

                    errorDiv += '</ul><p>Udfyld venligst felterne og send igen.</p></div>';

                    form.prepend($(errorDiv));

                    if (typeof callback === 'function') {
                        callback();
                    }
                }
            }
        });
    };

    formbuilder.clearErrors = function(form) {
        var rendererSettings = $(form).data('renderersettings');

        $('.' + rendererSettings.ParentGroupClass + '-group', form).removeClass(rendererSettings.ErrorClass);
        $('.' + rendererSettings.ErrorNotificationClass, form).remove();

        form.data('error', false);
    };

    var validation = function(form, callback) {
        var fileElements = $('input[type=file]', form);
        if (fileElements.length > 0) {
            return;
        }

        formbuilder.clearErrors(form);
        formbuilder.validate(form, form.serializeArray(), callback);
    };

    var getFormFieldValue = function(form, fieldName) {
        var rendererSettings = $(form).data('renderersettings');

        var field = $('[name="' + fieldName + '"]');

        if (field.is(':radio')) {
            field = field.filter(":checked");
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
            return field.is(':checked').toString();
        }

        return field.val();
    };

    var isMatch = function(form, field, itm) {
        var fieldValue = getFormFieldValue(form, field);
        if (fieldValue) {
            var regexp = new RegExp('^' + itm + '$', 'i');

            return regexp.test(fieldValue);
        }

        return false;
    };

    var showFunction = function(form, json) {
        var show = false;

        $.each(json, function(ix, itm) {
            var field = itm.field;
            var fieldValid = false;

            $.each(itm.value, function(ix, itm) {
                fieldValid = fieldValid || isMatch(form, field, itm);
            });

            if (!fieldValid) {
                show = false;

                return false;
            }

            show = fieldValid;
        });

        return show;
    };

    var dependecyFunction = function(form) {
        // We loop this function twice so controls that are depenent on eachother on the same "level" gets 
        // shown correctly. If we don't do this, we risk a control not showing up because its depentent on 
        // the next control, which is hidden during the first run.
        for (var i = 0; i < 2; i++) {
            $('[data-dependency]', form).each(function() {
                var $itm = $(this);
                var json = $itm.data('dependency');
                var show = showFunction(form, json);

                if (!show) {
                    $itm.hide();
                } else {
                    $itm.show();
                }
            });
        }
    };

    $(document).ready(function() {
        var forms = $('form[class^="form formbuilder-"]');

        $(document).on('change', 'form[class^="form formbuilder-"] :input', function() {
            var form = $(this).parents('form');

            dependecyFunction(form);
        });

        forms.each(function() {
            var form = $(this);

            dependecyFunction(form);
        });

        var btnSelector = 'input[type=submit]';

        if (window.Ladda) {
            $.each(forms, function(ix, form) {
                var submitButtons = $(btnSelector, form);

                $.each(submitButtons, function(ix, btn) {
                    btn = $(btn);
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
                    if (clickedButton.is(":disabled")) {
                        l = window.Ladda.create(clickedButton[0]);

                        l.start();
                    }
                }, 500);
            }

            validation(form, function() {
                clickedButton.attr('disabled', false);
                hiddenField.remove();

                if (l !== undefined) {
                    l.stop();
                }

                window.scrollTo(0, 0);

                e.preventDefault();
            });
        });
    });
})(jQuery, window, document);