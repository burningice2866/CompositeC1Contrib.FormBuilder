using System.Collections.Generic;
using System.Linq;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class FileMimeTypeValidatorAttribute : FormValidationAttribute
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
            var value = Enumerable.Empty<FormFile>();

            if (field.Value != null) 
            {
                if (field.ValueType == typeof(FormFile))
                {
                    value = new[] { (FormFile)field.Value };
                }
                else
                {
                    value = (IEnumerable<FormFile>)field.Value;
                }
            }           

            return new FormValidationRule(new[] { field.Name }, Message)
            {
                Rule = () =>
                {
                    foreach (var f in value)
                    {
                        var mimeType = f.ContentType;
                        if (!MimeTypes.Contains(mimeType))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            };
        }
    }
}
