(function($, formbuilder, window, document, undefined) {
    var validation = function(form, step) {
        var formSerialized = form.serializeArray();
        var formData = [];

        $.each(formSerialized, function() {
            var name = this.name;
            var stepPrepend = 'step-' + step + '-';

            if (name.indexOf(stepPrepend) === 0) {
                var stepPrependLength = stepPrepend.length;

                this.name = name.substr(stepPrependLength, name.length - stepPrependLength);
                formData.push(this);
            }
        });

        formData.push({
            name: '__type',
            value: $('input[name=step_' + step + ']', form).val()
        });

        formbuilder.clearErrors(form);
        formbuilder.validate(form, formData);
    };

    $(document).ready(function() {
        var forms = $('form[class^="form formwizard-"]');

        $(document).on('change', 'form[class^="form formwizard-"] :input', function() {
            var form = $(this).parents('form');

            formbuilder.dependecyFunction(form);
        });

        forms.each(function() {
            var form = $(this);

            formbuilder.dependecyFunction(form);
        });

        if (window.Ladda) {
            $.each(forms, function() {
                var form = $(this);
                var submitButtons = $('input[type="submit"]:not([data-nextstep])', form);

                $.each(submitButtons, function() {
                    var btn = $(this);
                    var val = btn.val();
                    var html = '<button class="ladda-button btn btn-primary" data-style="expand-right" type="submit" name="SubmitForm" value="' + val + '">' + val + '</button>';

                    btn.replaceWith(html);
                });
            });
        }

        forms.on('click', 'input[type="submit"], button', function(e) {
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
                    window.formbuilderWizard.navigateTo(nextStep, form);
                } else {
                    if (window.Ladda && button.hasClass('ladda-button')) {
                        setTimeout(function() {
                            var l = window.Ladda.create(button[0]);

                            l.start();
                        }, 500);
                    }

                    form.submit();
                }
            } else {
                window.scrollTo(0, 0);
            }
        });
    });

    window.formbuilderWizard = window.formbuilderWizard || {};

    window.formbuilderWizard.navigateTo = function(nextStep, form) {
        var containers = $('.js-formwizard-step[data-step]', form);
        var nextStepContainer = $('.js-formwizard-step[data-step=' + nextStep + ']', form);

        containers.hide();
        nextStepContainer.show();

        window.scrollTo(0, 0);

        var event = $.Event('formbuilder.wizard.navigate.success');
        event.nextStep = nextStep;

        form.trigger(event);
    }
})(jQuery, window.formbuilder, window, document);