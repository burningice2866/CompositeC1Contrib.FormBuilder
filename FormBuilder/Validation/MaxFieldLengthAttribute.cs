using System;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class MaxFieldLengthAttribute : FormValidationAttribute
    {
        public int Length { get; private set; }

        public MaxFieldLengthAttribute(string message, int length)
            : base(message)
        {
            Length = length;
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            var value = (string)field.Value;

            return new FormValidationRule(new[] { field.Name }, String.Format(Message, Length))
            {
                Rule = () =>
                {
                    if (String.IsNullOrEmpty(value) && !field.IsRequired)
                    {
                        return true;
                    }
                    else
                    {
                        return value.Length <= Length;
                    }
                }
            };
        }
    }
}
