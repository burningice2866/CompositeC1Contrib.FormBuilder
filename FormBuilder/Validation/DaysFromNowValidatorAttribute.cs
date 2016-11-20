using System;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class DaysFromNowValidatorAttribute : FormValidationAttribute
    {
        private readonly int _daysFromNow;

        public DaysFromNowValidatorAttribute(int daysFromNow) : this(null, daysFromNow) { }

        public DaysFromNowValidatorAttribute(string message, int daysFromNow)
            : base(message)
        {
            _daysFromNow = daysFromNow;
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            DateTime? dt = null;

            if (field.ValueType == typeof(DateTime))
            {
                dt = (DateTime)field.Value;
            }
            else if (field.ValueType == typeof(DateTime?))
            {
                dt = (DateTime?)field.Value;
            }

            var rule = CreateRule(field, () =>
            {
                if (!dt.HasValue)
                {
                    return !field.IsRequired;
                }

                return (dt.Value - DateTime.Now).Days >= _daysFromNow;
            });

            rule.FormatMessage = m => String.Format(m, _daysFromNow);

            return rule;
        }
    }
}
