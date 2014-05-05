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
        var forms = $('form.formwizard');

        if (window.Ladda) {
            $.each(forms, function (ix, form) {
                var submitButtons = $('input[type="submit"]:not([data-nextstep])', form);

                $.each(submitButtons, function (ix, btn) {
                    btn = $(btn);
                    var val = btn.val();
                    var html = '<button class="ladda-button btn btn-primary" data-style="expand-right" type="submit" name="SubmitForm" value="' + val + '">' + val + '</button>';

                    btn.replaceWith(html);
                });
            });
        }

        $('.formwizard').on('click', 'input[type="submit"], button', function (e) {
            e.preventDefault();
            var button = $(this);
            var container = button.closest('[data-step]');
            var form = container.closest('form');
            var step = container.data('step');
            var nextStep = button.data('nextstep');

            formbuilder.clearErrors(form);

            if (nextStep === undefined || nextStep > step) {
                validation(form, step);
            }

            if (form.data('error') === false) {
                if (nextStep !== undefined) {
                    container.hide();
                    $('.js-formwizard-step[data-step=' + nextStep + ']', form).show();
                }
                else
                {
                    if (window.Ladda && button.hasClass('ladda-button')) {
                        setTimeout(function () {
                            var l = window.Ladda.create(button[0]);

                            l.start();
                        }, 500);
                    }

                    form.submit();
                }
            }
        });
    });
})(jQuery, window.formbuilder, window, document);