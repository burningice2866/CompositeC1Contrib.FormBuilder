using System.Collections.Generic;
using System.Linq;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public abstract class BaseFileValidatorAttribute : FormValidationAttribute
    {
        protected BaseFileValidatorAttribute() { }

        protected BaseFileValidatorAttribute(string message) : base(message) { }

        protected IEnumerable<FormFile> GetFiles(FormField field)
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

            return value;
        }
    }
}
