﻿(function ($, window, document, undefined) {
    var getFormFieldValue = function (fieldName) {
        var field = $('[name="' + fieldName + '"]');

        if (field.is(':radio')) {
            field = field.filter(":checked");
        }

        var element = field.eq(0);
        var hidden = false;

        while (element.length > 0) {
            if (element.hasClass('control-group')) {
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

    var dependecyFunction = function () {
        // We loop this function twice so controls that are depenent on eachother on the same "level" gets 
        // shown correctly. If we don't do this, we risk a control not showing up because its depentent on 
        // the next control, which is hidden during the first run.
        for (var i = 0; i < 2; i++) {
            $('[data-dependency]').each(function (ix, itm) {
                var $itm = $(itm);
                var json = $itm.data('dependency');
                var show = showFunction(json);

                if (!show) {
                    $itm.hide();
                } else {
                    $itm.show();
                }
            });
        }
    };

    var isMatch = function (field, itm) {
        var fieldValue = getFormFieldValue(field);
        if (fieldValue) {
            var regexp = new RegExp('^' + itm + '$', 'i');

            return regexp.test(fieldValue);
        }

        return false;
    };

    var showFunction = function (json) {
        var show = false;

        $.each(json, function (ix, itm) {
            var field = itm.field;
            var fieldValid = false;

            $.each(itm.value, function (ix, itm) {
                fieldValid = fieldValid || isMatch(field, itm);
            });

            if (!fieldValid) {
                show = false;

                return show;
            }

            show = fieldValid;
        });

        return show;
    };

    $(document).ready(function () {
        $(':input').change(dependecyFunction);

        dependecyFunction();

        var forms = $('form[class^="form formbuilder-"]');
        var btnSelector = 'input[type=submit]';

        if (window.Ladda) {
            $.each(forms, function (ix, form) {
                var submitButtons = $(btnSelector, form);

                $.each(submitButtons, function (ix, btn) {
                    btn = $(btn);
                    var val = btn.val();
                    var html = '<button class="ladda-button btn btn-primary" data-style="expand-right" type="submit" name="SubmitForm" value="' + val + '">' + val + '</button>';

                    btn.replaceWith(html);
                });
            });

            btnSelector = 'button[type=submit]';
        }

        forms.on('click', btnSelector, function () {
            var btn = $(this);
            var form = btn.parents('form');
            var submitButtons = $(btnSelector, form);

            submitButtons.removeAttr('clicked');
            btn.attr('clicked', true);
        });

        forms.on('submit', function () {
            var form = $(this);
            var clickedButton = $(btnSelector + '[clicked=true]', form);

            clickedButton.attr('disabled', true);

            if (window.Ladda) {
                setTimeout(function () {
                    var l = window.Ladda.create(clickedButton[0]);

                    l.start();
                }, 500);
            }
        });
    });
})(jQuery, window, document);