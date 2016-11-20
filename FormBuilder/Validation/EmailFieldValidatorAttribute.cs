using System.Text.RegularExpressions;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class EmailFieldValidatorAttribute : RegexValidatorAttribute
    {
        private const string Pattern = @"(?i)\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b";
        private static readonly Regex Regex = new Regex(Pattern, RegexOptions.Compiled);

        public EmailFieldValidatorAttribute() : this(null) { }
        public EmailFieldValidatorAttribute(string message) : base(message, Pattern) { }

        public static bool Validate(string email)
        {
            return !Regex.IsMatch(email);
        }
    }
}
