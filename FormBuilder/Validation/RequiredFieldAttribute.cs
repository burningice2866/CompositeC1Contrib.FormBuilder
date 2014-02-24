using System;
using System.Collections.Generic;
using System.Linq;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class RequiredFieldAttribute : FormValidationAttribute
    {
        public RequiredFieldAttribute(string message) : base(message) { }

        public override FormValidationRule CreateRule(FormField field)
        {
            var valueType = field.ValueType;
            var value = field.Value;

            return new FormValidationRule(new[] { field.Name }, Message)
            {
                Rule = () =>
                {
                    if (valueType == typeof(string))
                    {
                        return !String.IsNullOrWhiteSpace((string)value);
                    }

                    if (valueType == typeof(bool))
                    {
                        return (bool)value;
                    }

                    if (valueType == typeof(int))
                    {
                        return (int)value > 0;
                    }

                    if (valueType == typeof(int?))
                    {
                        return ((int?)value).HasValue;
                    }

                    if (valueType == typeof(DateTime))
                    {
                        return (DateTime)value > DateTime.MinValue;
                    }
                    if (valueType == typeof(DateTime?))
                    {
                        return ((DateTime?)value).HasValue;
                    }

                    if (valueType == typeof(FormFile))
                    {
                        return ((FormFile)value).ContentLength > 0;
                    }
                    if (valueType == typeof(IEnumerable<FormFile>))
                    {
                        return ((IEnumerable<FormFile>)value).Any(f => f.ContentLength > 0);
                    }

                    if (valueType == typeof(IEnumerable<string>))
                    {
                        return ((IEnumerable<string>)value).Any(f => !String.IsNullOrWhiteSpace(f));
                    }

                    return value != null;
                }
            };
        }
    }
}
