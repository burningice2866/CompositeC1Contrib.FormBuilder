(function ($, formbuilder, window, document, undefined) {
    var validation = function (form, step) {
        var formSerialized = form.serializeArray();
        var formData = [];

        $.each(formSerialized, function (i, itm) {
            var name = itm.name;
            var stepPrepend = 'step-' + step + '-';

            if (name.indexOf(stepPrepend) === 0) {
                var stepPrependLength = stepPrepend.length;

                itm.name = name.substr(stepPrependLength, name.length - stepPrependLength);
                formData.push(itm);
            }
        });

        formData.push({
            name: '__type',
            value: $('input[name=step_' + step + ']', form).val()
        });

        formbuilder.clearErrors(form);
        formbuilder.validate(form, formData);
    };

    $(document).ready(function () {
        $('.formwizard').on('click', 'button', function (e) {
            e.preventDefault();

            var button = $(this);
            var container = button.parent();
            var form = container.parent();
            var step = container.data('step');
            var nextStep = button.data('nextstep');

            formbuilder.clearErrors(form);

            if (nextStep > step) {
                validation(form, step);
            }

            if (form.data('error') === false) {
                if (nextStep !== undefined) {
                    container.hide();
                    $('.js-formwizard-step[data-step=' + nextStep + ']', form).show();
                }
            }
        });
    });
})(jQuery, window.formbuilder, window, document);