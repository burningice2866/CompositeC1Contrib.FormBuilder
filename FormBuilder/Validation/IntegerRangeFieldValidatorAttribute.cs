﻿using System;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class IntegerRangeFieldValidatorAttribute : FormValidationAttribute
    {
        private readonly int _minValue;
        private readonly int _maxValue;

        public IntegerRangeFieldValidatorAttribute(int minValue, int maxValue) : this(null, minValue, maxValue) { }

        public IntegerRangeFieldValidatorAttribute(string message, int minValue, int maxValue)
            : base(message)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            var rule = CreateRule(field, () =>
            {
                var s = field.OwningForm.SubmittedValues[field.Name];
                var i = 0;

                if (!int.TryParse(s, out i))
                {
                    return !field.IsRequired;
                }

                if (i < _minValue)
                {
                    return false;
                }

                if (i > _maxValue)
                {
                    return false;
                }

                return true;
            });

            rule.FormatMessage = m => String.Format(m, _minValue, _maxValue);

            return rule;
        }
    }
}
