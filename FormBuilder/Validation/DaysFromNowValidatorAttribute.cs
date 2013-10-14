using System;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class DaysFromNowValidatorAttribute : FormValidationAttribute
    {
        private int _daysFromNow;

        public DaysFromNowValidatorAttribute(string message, int daysFromNow)
            : base(String.Format(message, daysFromNow))
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

            return new FormValidationRule(new[] { field.Name }, Message)
            {
                Rule = () =>
                {
                    if (!dt.HasValue)
                    {
                        return !field.IsRequired;
                    }

                    return (dt.Value - DateTime.Now).Days >= _daysFromNow;
                }
            };
        }
    }
}
