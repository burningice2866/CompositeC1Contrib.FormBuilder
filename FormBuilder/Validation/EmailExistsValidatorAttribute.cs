using System.Linq;
using System.Web.Security;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class EmailExistsValidatorAttribute : FormValidationAttribute
    {
        private readonly bool _onlyApproved = true;

        public EmailExistsValidatorAttribute(string message) : this(message, true) { }

        public EmailExistsValidatorAttribute(string message, bool onlyApproved)
            : base(message)
        {
            _onlyApproved = onlyApproved;
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            var value = (string)field.Value;

            return new FormValidationRule(new[] { field.Name }, Message)
            {
                Rule = () => Membership.FindUsersByEmail(value).Cast<MembershipUser>().Any(u => !_onlyApproved || u.IsApproved)
            };
        }
    }
}
