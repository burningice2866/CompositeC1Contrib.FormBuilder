using System;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class IntegerFieldValidatorAttribute : FormValidationAttribute
    {
        public IntegerFieldValidatorAttribute() { }

        public IntegerFieldValidatorAttribute(string message) : base(message) { }

        public override FormValidationRule CreateRule(FormField field)
        {
            return CreateRule(field, () =>
            {
                var s = field.OwningForm.SubmittedValues[field.Name];
                if (String.IsNullOrEmpty(s))
                {
                    return !field.IsRequired;
                }

                int i;
                if (!int.TryParse(s, out i))
                {
                    return false;
                }

                return true;
            });
        }
    }
}
