using System;
using System.Collections.Generic;

using CompositeC1Contrib.FormBuilder;
using CompositeC1Contrib.FormBuilder.Validation;

using NUnit.Framework;

namespace FormBuilder.Test.Validation
{
    public abstract class BaseValidatorTest<T>
    {
        private FormField _field;
        private readonly FormValidationAttribute _validator;

        protected BaseValidatorTest(Func<FormValidationAttribute> validatorCreator)
        {
            _validator = validatorCreator();
        }

        [TestFixtureSetUp]
        public void Init()
        {
            var form = new FormModel("test.test");

            _field = new FormField(form, "test", typeof(T), new List<Attribute>());
        }

        protected FormValidationRule CreateRule(T value)
        {
            _field.Value = value;

            return _validator.CreateRule(_field);
        }
    }
}
