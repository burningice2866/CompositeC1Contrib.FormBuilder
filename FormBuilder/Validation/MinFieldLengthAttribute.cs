using System;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class MinFieldLengthAttribute : FormValidationAttribute
    {
        private int _length;

        public MinFieldLengthAttribute(string message, int length)
            : base(message)
        {
            _length = length;
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            var value = (string)field.Value;

            return new FormValidationRule(new[] { field.Name }, String.Format(Message, _length))
            {
                Rule = () =>
                {
                    if (String.IsNullOrEmpty(value) && !field.IsRequired)
                    {
                        return true;
                    }
                    else
                    {
                        return value.Length >= _length;
                    }
                }
            };
        }
    }
}
