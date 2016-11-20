using System;
using System.Collections.Generic;

using CompositeC1Contrib.FormBuilder;
using CompositeC1Contrib.FormBuilder.Validation;

using NUnit.Framework;

namespace FormBuilder.Test.Validation
{
    public abstract class BaseValidatorTest<T>
    {
        private FormFieldModel _field;
        private readonly FormValidationAttribute _validator;

        protected BaseValidatorTest(Func<FormValidationAttribute> validatorCreator)
        {
            _validator = validatorCreator();
        }

        [TestFixtureSetUp]
        public void Init()
        {
            var model = new FormModel("test.test");

            _field = new FormFieldModel(model, "test", typeof(T), new List<Attribute>());
        }

        protected FormValidationRule CreateRule(T value)
        {
            var form = new Form(_field.OwningForm);

            var field = new FormField(_field, form)
            {
                Value = value
            };

            return _validator.CreateRule(field);
        }
    }
}
