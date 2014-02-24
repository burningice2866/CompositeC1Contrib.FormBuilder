using System;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class MinFieldLengthAttribute : FormValidationAttribute
    {
        public int Length { get; private set; }

        public MinFieldLengthAttribute(string message, int length)
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

                    return value.Length >= Length;
                }
            };
        }
    }
}
