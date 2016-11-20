using System;
using System.Linq;
using System.Web.Security;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class EmailInUseValidatorAttribute : FormValidationAttribute
    {
        private readonly bool _onlyApproved = true;

        public EmailInUseValidatorAttribute() : this(null) { }

        public EmailInUseValidatorAttribute(string message) : this(message, true) { }

        public EmailInUseValidatorAttribute(bool onlyApproved) : this(null, onlyApproved) { }

        public EmailInUseValidatorAttribute(string message, bool onlyApproved)
            : base(message)
        {
            _onlyApproved = onlyApproved;
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            var value = (string)field.Value;

            return CreateRule(field, () =>
            {
                var currentUser = Membership.GetUser();
                if (currentUser != null)
                {
                    if (!String.Equals(value, currentUser.Email, StringComparison.OrdinalIgnoreCase))
                    {
                        return !IsEmailInUse(value, _onlyApproved);
                    }

                    return true;
                }

                return !IsEmailInUse(value, _onlyApproved);
            });
        }

        public static bool IsEmailInUse(string email, bool onlyApproved)
        {
            return Membership.FindUsersByEmail(email).Cast<MembershipUser>().Any(u => !onlyApproved || u.IsApproved);
        }
    }
}
