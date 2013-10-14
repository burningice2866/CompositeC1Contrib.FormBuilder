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
            var value = field.Value;

            return new FormValidationRule(new[] { field.Name }, Message)
            {
                Rule = () =>
                {
                    if (value is string)
                    {
                        return !String.IsNullOrWhiteSpace((string)value);
                    }
                    
                    else if (value is bool)
                    {
                        return (bool)value;
                    }
                    
                    else if (value is int)
                    {
                        return (int)value > 0;
                    }
                    else if (value is int?)
                    {
                        return ((int?)value).HasValue;
                    }

                    else if (value is DateTime)
                    {
                        return (DateTime)value > DateTime.MinValue;
                    }
                    else if (value is DateTime?)
                    {
                        return ((DateTime?)value).HasValue;
                    }
                    
                    else if (value is FormFile)
                    {
                        return ((FormFile)value).ContentLength > 0;
                    }
                    else if (value is IEnumerable<FormFile>)
                    {
                        return ((IEnumerable<FormFile>)value).Any(f => f.ContentLength > 0);
                    }
                    
                    else
                    {
                        return value != null;
                    };
                }
            };
        }
    }
}
