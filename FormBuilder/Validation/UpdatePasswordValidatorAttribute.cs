using System;
using System.Linq;
using System.Web.Security;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class UpdatePasswordValidatorAttribute : FormValidationAttribute
    {
        private string _newPasswordField;
        private string _repeatNewPasswordField;

        public UpdatePasswordValidatorAttribute(string message, string newPasswordField, string repeatNewPasswordField)
            : base(message)
        {
            _newPasswordField = newPasswordField;
            _repeatNewPasswordField = repeatNewPasswordField;
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            var value = (string)field.Value;
            var newPassword = (string)field.OwningForm.Fields.Single(f => f.Name == _newPasswordField).Value;
            var repeatNewPassword = (string)field.OwningForm.Fields.Single(f => f.Name == _repeatNewPasswordField).Value;

            return new FormValidationRule(new[] { field.Name, _newPasswordField, _repeatNewPasswordField }, Message)
            {
                Rule = () =>
                {
                    if (!String.IsNullOrEmpty(newPassword))
                    {
                        var user = Membership.GetUser();

                        return Membership.ValidateUser(user.UserName, value);
                    }

                    if (!String.IsNullOrEmpty(value))
                    {
                        return field.OwningForm.IsValid(new[] { _newPasswordField, _repeatNewPasswordField });
                    }

                    return true;
                }
            };
        }
    }
}
