using System;
using System.Text.RegularExpressions;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class RegexValidatorAttribute : FormValidationAttribute
    {
        private readonly Regex _regex;

        public string RegexPattern { get; private set; }

        public RegexValidatorAttribute(string message, string regexPattern)
            : base(message)
        {
            RegexPattern = regexPattern;
            _regex = new Regex(RegexPattern);
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
