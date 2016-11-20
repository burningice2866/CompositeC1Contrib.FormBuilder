using System;
using System.Web.Security;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class UpdatePasswordValidatorAttribute : FormValidationAttribute
    {
        private readonly string _newPasswordField;
        private readonly string _repeatNewPasswordField;

        public UpdatePasswordValidatorAttribute(string newPasswordField, string repeatNewPasswordField) : this(null, newPasswordField, repeatNewPasswordField) { }

        public UpdatePasswordValidatorAttribute(string message, string newPasswordField, string repeatNewPasswordField)
            : base(message)
        {
            _newPasswordField = newPasswordField;
            _repeatNewPasswordField = repeatNewPasswordField;
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            var value = (string)field.Value;
            var newPassword = (string)field.OwningForm.Fields.Get(_newPasswordField).Value;

            return CreateRule(field, new[] { field.Name, _newPasswordField, _repeatNewPasswordField }, () =>
            {
                if (!String.IsNullOrEmpty(newPassword))
                {
                    var user = Membership.GetUser();

                    return Membership.ValidateUser(user.UserName, value);
                }

                return String.IsNullOrEmpty(value) || field.OwningForm.IsValid(new[] { _newPasswordField, _repeatNewPasswordField });
            });
        }
    }
}
