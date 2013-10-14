namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class IntegerFieldValidatorAttribute : FormValidationAttribute
    {
        public IntegerFieldValidatorAttribute(string message) : base(message) { }

        public override FormValidationRule CreateRule(FormField field)
        {
            return new FormValidationRule(new[] { field.Name }, Message)
            {
                Rule = () =>
                {
                    var s = field.OwningForm.SubmittedValues[field.Name];
                    var i = 0;

                    if (!int.TryParse(s, out i))
                    {
                        return !field.IsRequired;
                    }

                    return true;
                }
            };
        }
    }
}
