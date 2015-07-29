using System;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class DecimalFieldValidatorAttribute : FormValidationAttribute
    {
        public DecimalFieldValidatorAttribute(string message) : base(message) { }

        public override FormValidationRule CreateRule(FormField field)
        {
            return new FormValidationRule(new[] { field.Name }, Message)
            {
                Rule = () =>
                {
                    var s = field.OwningForm.SubmittedValues[field.Name];
                    if (String.IsNullOrEmpty(s))
                    {
                        return !field.IsRequired;
                    }

                    decimal i;
                    if (!decimal.TryParse(s, out i))
                    {
                        return false;
                    }

                    return true;
                }
            };
        }
    }
}
