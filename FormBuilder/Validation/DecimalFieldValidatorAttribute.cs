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
                    var i = 0m;

                    if (!decimal.TryParse(s, out i))
                    {
                        return !field.IsRequired;
                    }

                    return true;
                }
            };
        }
    }
}
