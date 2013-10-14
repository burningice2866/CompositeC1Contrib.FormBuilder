namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class IntegerRangeFieldValidatorAttribute : FormValidationAttribute
    {
        private int _minValue;
        private int _maxValue;

        public IntegerRangeFieldValidatorAttribute(string message, int minValue, int maxValue)
            : base(message)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

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
                    else
                    {
                        if (i < _minValue)
                        {
                            return false;
                        }

                        if (i > _maxValue)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            };
        }
    }
}
