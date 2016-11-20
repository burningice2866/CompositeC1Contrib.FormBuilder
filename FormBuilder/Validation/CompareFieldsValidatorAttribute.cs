namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class CompareFieldsValidatorAttribute : FormValidationAttribute
    {
        private readonly string _fieldToCompare;
        private readonly CompareOperator _op;

        public CompareFieldsValidatorAttribute(string fieldToCompare, CompareOperator op) : this(null, fieldToCompare, op) { }

        public CompareFieldsValidatorAttribute(string message, string fieldToCompare, CompareOperator op)
            : base(message)
        {
            _fieldToCompare = fieldToCompare;
            _op = op;
        }

        public override FormValidationRule CreateRule(FormField field)
        {
            var value = field.Value;
            var valueToCompare = field.OwningForm.Fields.Get(_fieldToCompare).Value;

            return CreateRule(field, new[] { field.Name, _fieldToCompare }, () =>
            {
                switch (_op)
                {
                    case CompareOperator.Equal: return value.Equals(valueToCompare);
                    case CompareOperator.NotEqual: return !value.Equals(valueToCompare);
                }

                return true;
            });
        }
    }
}
