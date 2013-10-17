using System;
using System.Text.RegularExpressions;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class RegexValidatorAttribute : FormValidationAttribute
    {
        private static Regex _regex;

        public string Regex { get; private set; }

        public RegexValidatorAttribute(string message, string regex)
            : base(message)
        {
            Regex = regex;
            _regex = new Regex(Regex, RegexOptions.Compiled);
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
