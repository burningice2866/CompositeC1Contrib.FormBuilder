using System;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class EqualsConstantValidatorAttribute : FormValidationAttribute
    {
        readonly object _constant;

        public EqualsConstantValidatorAttribute(object constant) : this(null, constant) { }

        public EqualsConstantValidatorAttribute(string message, object constant)
            : base(message)
        {
            _constant = constant;
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            var value = field.Value;

            var rule = CreateRule(field, () =>
            {
                return value == _constant;
            });

            rule.FormatMessage = m => String.Format(m, _constant);

            return rule;
        }
    }
}
