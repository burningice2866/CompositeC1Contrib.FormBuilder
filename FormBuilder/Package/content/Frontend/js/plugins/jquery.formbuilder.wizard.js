(function($, window, document, formbuilder, undefined) {
    var selector = 'form[class^="form formwizard-"]';

    formbuilder.wizard = {};

    var validation = function(form, step) {
        var dfd = $.Deferred();

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

        form.formbuilder('clearErrors');

        form.formbuilder('validate', formData).fail(function() {
            dfd.reject();
        }).done(function() {
            dfd.resolve();
        });

        return dfd.promise();
    };

    var _init = function(forms) {
        forms.on('change', ':input', function() {
            var form = $(this).parents('form');

            form.formbuilder('dependecyFunction');
        });

        forms.each(function() {
            var form = $(this);

            form.formbuilder('dependecyFunction');
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

            form.formbuilder('clearErrors');

            var step = container.data('step');
            var nextStep = button.data('nextstep');

            if (nextStep === undefined || nextStep > step) {
                validation(form, step).fail(function() {
                    window.scrollTo(0, 0);
                }).done(function() {
                    if (nextStep !== undefined) {
                        form.formbuilder('wizard.navigateTo', nextStep);
                    } else {
                        if (window.Ladda && button.hasClass('ladda-button')) {
                            setTimeout(function() {
                                var l = window.Ladda.create(button[0]);

                                l.start();
                            }, 500);
                        }

                        form.submit();
                    }
                });
            }
        });
    };

    var _navigateTo = function(form, nextStep) {
        var containers = $('.js-formwizard-step[data-step]', form);
        var nextStepContainer = $('.js-formwizard-step[data-step=' + nextStep + ']', form);

        containers.hide();
        nextStepContainer.show();

        window.scrollTo(0, 0);

        var event = $.Event('formbuilder.wizard.navigate.success');
        event.nextStep = nextStep;

        form.trigger(event);
    };

    formbuilder.wizard.navigateTo = _navigateTo;

    formbuilder.initializers.push({
        selector: selector,
        initializer: _init
    });

    $(document).ready(function() {
        var forms = $(selector);

        forms.formbuilder();
    });
})(jQuery, window, document, window.formbuilder);