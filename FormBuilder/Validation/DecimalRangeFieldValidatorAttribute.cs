using System;


namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class DecimalRangeFieldValidatorAttribute : FormValidationAttribute
    {
        private readonly decimal _minValue;
        private readonly decimal _maxValue;

        public DecimalRangeFieldValidatorAttribute(decimal minValue) : this(null, minValue) { }

        public DecimalRangeFieldValidatorAttribute(string message, decimal minValue) : this(message, minValue, decimal.MaxValue) { }

        public DecimalRangeFieldValidatorAttribute(decimal minValue, decimal maxValue) : this(null, minValue, maxValue) { }

        public DecimalRangeFieldValidatorAttribute(string message, decimal minValue, decimal maxValue)
            : base(message)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            var rule = CreateRule(field, () =>
            {
                var s = field.OwningForm.SubmittedValues[field.Name];
                var i = 0m;

                if (!decimal.TryParse(s, out i))
                {
                    return !field.IsRequired;
                }

                if (i < _minValue)
                {
                    return false;
                }

                if (i > _maxValue)
                {
                    return false;
                }

                return true;
            });

            rule.FormatMessage = m => String.Format(m, _minValue, _maxValue);

            return rule;
        }
    }
}
