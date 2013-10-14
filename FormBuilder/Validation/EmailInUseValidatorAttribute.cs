using System;
using System.Linq;
using System.Web.Security;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class EmailInUseValidatorAttribute : FormValidationAttribute
    {
        private bool _onlyApproved = true;

        public EmailInUseValidatorAttribute(string message) : this(message, true) { }

        public EmailInUseValidatorAttribute(string message, bool onlyApproved)
            : base(message)
        {
            _onlyApproved = onlyApproved;
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            var value = (string)field.Value;

            return new FormValidationRule(new[] { field.Name }, Message)
            {
                Rule = () =>
                {
                    var currentUser = Membership.GetUser();
                    if (currentUser != null)
                    {
                        if (!String.Equals(value, currentUser.Email, StringComparison.OrdinalIgnoreCase))
                        {
                            return !IsEmailInUse(value, _onlyApproved);
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return !IsEmailInUse(value, _onlyApproved);
                    }
                }
            };
        }

        public static bool IsEmailInUse(string email, bool onlyApproved)
        {
            return Membership.FindUsersByEmail(email).Cast<MembershipUser>().Where(u =>
            {
                if (onlyApproved)
                {
                    return u.IsApproved;
                }

                return true;
            }).Any();
        }
    }
}
