using System.Web.Security;
using System.Linq;

using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class CredentialsValidatorAttribute : FormValidationAttribute
    {
        private readonly string _passwordField;

        public CredentialsValidatorAttribute(string message, string passwordField)
            : base(message)
        {
            _passwordField = passwordField;
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            var value = (string)field.Value;
            var password = (string)field.OwningForm.Fields.Single(f => f.Name == _passwordField).Value;

            return new FormValidationRule(new[] { field.Name, _passwordField }, Message)
            {
                Rule = () =>
                {
                    if (UserValidationFacade.IsLoggedIn())
                    {
                        return true;
                    }

                    var userName = Membership.GetUserNameByEmail(value);

                    return userName != null && Membership.ValidateUser(userName, password);
                }
            };
        }
    }
}
