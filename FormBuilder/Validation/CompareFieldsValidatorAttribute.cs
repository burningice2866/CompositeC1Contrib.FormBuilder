using System.Linq;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class CompareFieldsValidatorAttribute : FormValidationAttribute
    {
        private string _fieldToCompare;
        private CompareOperator _op;

        public CompareFieldsValidatorAttribute(string message, string fieldToCompare, CompareOperator op)
            : base(message)
        {
            _fieldToCompare = fieldToCompare;
            _op = op;
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            var value = field.Value;
            var valueToCompare = field.OwningForm.Fields.Single(f => f.Name == _fieldToCompare).Value;

            return new FormValidationRule(new[] { field.Name, _fieldToCompare }, Message)
            {
                Rule = () =>
                {
                    switch (_op)
                    {
                        case CompareOperator.Equal: return value.Equals(valueToCompare);
                        case CompareOperator.NotEqual: return !value.Equals(valueToCompare);
                    }

                    return true;
                }
            };
        }
    }
}
