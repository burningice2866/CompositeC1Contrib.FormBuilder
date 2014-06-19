using System.Linq;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class FileMimeTypeValidatorAttribute : BaseFileValidatorAttribute
    {
        public string[] MimeTypes { get; private set; }

        public FileMimeTypeValidatorAttribute(string message, string mimeType) : this(message, new[] { mimeType }) { }

        public FileMimeTypeValidatorAttribute(string message, params string[] mimeTypes)
            : base(message)
        {
            MimeTypes = mimeTypes;
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            var value = GetFiles(field);

            return new FormValidationRule(new[] { field.Name }, Message)
            {
                Rule = () => value.Select(f => f.ContentType).All(mimeType => MimeTypes.Contains(mimeType))
            };
        }
    }
}
