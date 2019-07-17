using System;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class MaxFieldLengthAttribute : FormValidationAttribute
    {
        public int Length { get; }

        public MaxFieldLengthAttribute(int length) : this(null, length) { }

        public MaxFieldLengthAttribute(string message, int length)
            : base(message)
        {
            Length = length;
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            var value = (string)field.Value;

            var rule = CreateRule(field, () =>
            {
                if (String.IsNullOrEmpty(value))
                {
                    return !field.IsRequired;
                }

                return value.Length <= Length;
            });

            rule.FormatMessage = m => String.Format(m, Length);

            return rule;
        }
    }
}
