using System.Globalization;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class DecimalRangeFieldValidatorAttribute : FormValidationAttribute
    {
        private decimal _minValue;
        private decimal _maxValue;

        public DecimalRangeFieldValidatorAttribute(string message, string minValue)
            : base(message)
        {
            _minValue = decimal.Parse(minValue, CultureInfo.InvariantCulture.NumberFormat);
            _maxValue = decimal.MaxValue;
        }

        public DecimalRangeFieldValidatorAttribute(string message, string minValue, string maxValue)
            : base(message)
        {
            _minValue = decimal.Parse(minValue, CultureInfo.InvariantCulture.NumberFormat);
            _maxValue = decimal.Parse(maxValue, CultureInfo.InvariantCulture.NumberFormat);
        }

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
