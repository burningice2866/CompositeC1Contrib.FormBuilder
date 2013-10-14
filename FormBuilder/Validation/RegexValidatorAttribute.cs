using System;
using System.Text.RegularExpressions;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class RegexValidatorAttribute : FormValidationAttribute
    {
        private static Regex _regex;

        public RegexValidatorAttribute(string message, string regex)
            : base(message)
        {
            _regex = new Regex(regex, RegexOptions.Compiled);
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            var value = (string)field.Value;

            return new FormValidationRule(new[] { field.Name }, Message)
            {
                Rule = () =>
                {
                    if (String.IsNullOrEmpty(value))
                    {
                        return !field.IsRequired;
                    }

                    return _regex.IsMatch(value);
                }
            };
        }
    }
}
