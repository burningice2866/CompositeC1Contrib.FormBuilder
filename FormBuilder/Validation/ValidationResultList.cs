using System.Collections.Generic;
using System.Linq;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class ValidationResultList : List<FormValidationRule>
    {
        public ValidationResultList() { }

        public ValidationResultList(IEnumerable<FormValidationRule> collection) : base(collection) { }

        public void Add(string id, string message)
        {
            Add(new FormValidationRule(new[] { id }, message));
        }

        public static ValidationResultList GetFormValidationResult(IEnumerable<FormValidationRule> rules, bool skipMultipleFieldsRules)
        {
            var failingRules = rules.Where(r =>
            {
                if (skipMultipleFieldsRules)
                {
                    return r.AffectedFormIds.Count() == 1;
                }

                return true;
            }).Where(r => r.IsInvalid());

            return new ValidationResultList(failingRules);
        }
    }
}
