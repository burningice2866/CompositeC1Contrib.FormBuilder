(function ($, document, undefined) {
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
    }

    var dependecyFunction = function () {
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
    };

    var isMatch = function (field, itm) {
        var fieldValue = getFormFieldValue(field);
        if (fieldValue) {
            var regexp = new RegExp('^' + itm + '$', 'i');

            return regexp.test(fieldValue)
        }

        return false;
    }

    var showFunction = function (json) {
        var show = false;

        $.each(json, function (ix, itm) {
            var field = itm.field;
            var fieldValid = false

            $.each(itm.value, function (ix, itm) {
                fieldValid = fieldValid || isMatch(field, itm);
            });

            if (!fieldValid) {
                show = false;

                return false;
            }

            show = fieldValid;
        });

        return show;
    };

    $(document).ready(function () {
        $(':input').change(dependecyFunction);

        dependecyFunction();
    });
})(jQuery, document);