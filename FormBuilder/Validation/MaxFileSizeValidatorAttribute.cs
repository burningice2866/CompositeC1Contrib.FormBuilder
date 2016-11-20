using System;
using System.Linq;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class MaxFileSizeValidatorAttribute : BaseFileValidatorAttribute
    {
        public double Size { get; private set; }

        public MaxFileSizeValidatorAttribute(double fileSize) : this(null, fileSize) { }

        public MaxFileSizeValidatorAttribute(string message, double fileSize)
            : base(message)
        {
            Size = fileSize;
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            var value = GetFiles(field);

            var rule = CreateRule(field, () =>
            {
                return value.Sum(f => f.ContentLength) <= Size;
            });

            rule.FormatMessage = m => String.Format(m, Size);

            return rule;
        }
    }
}
