using System.Linq;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class MaxFileSizeValidatorAttribute : BaseFileValidatorAttribute
    {
        public double Size { get; private set; }

        public MaxFileSizeValidatorAttribute(string message, double fileSize)
            : base(message)
        {
            Size = fileSize;
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            var value = GetFiles(field);

            return new FormValidationRule(new[] { field.Name }, Message)
            {
                Rule = () => value.Sum(f => f.ContentLength) <= Size
            };
        }
    }
}
