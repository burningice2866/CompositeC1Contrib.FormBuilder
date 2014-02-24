namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class EqualsConstantValidatorAttribute : FormValidationAttribute
    {
        readonly object _constant;

        public EqualsConstantValidatorAttribute(string message, object constant)
            : base(message)
        {
            _constant = constant;
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            var value = field.Value;

            return new FormValidationRule(new[] { field.Name }, Message)
            {
                Rule = () => (value == _constant)
            };
        }
    }
}
