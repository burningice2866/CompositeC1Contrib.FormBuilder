using System;
using System.Collections.Generic;

using Composite;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class FormValidationRule
    {
        private Func<bool> _rule;

        public IEnumerable<string> AffectedFormIds { get; private set; }
        public Func<string, string> FormatMessage { get; set; }

        public string ValidationMessage { get; private set; }

        [Obsolete("Pass the rule into the constructor")]
        public Func<bool> Rule
        {
            get { return _rule; }
            set { _rule = value; }
        }

        public FormValidationRule(IEnumerable<string> affectedFormIds, string validationMessage)
            : this(affectedFormIds, validationMessage, () => true) { }

        public FormValidationRule(IEnumerable<string> affectedFormIds, string validationMessage, Func<bool> rule)
        {
            Verify.ArgumentNotNull(affectedFormIds, "affectedFormIds");
            Verify.ArgumentNotNull(validationMessage, "validationMessage");

            _rule = rule;

            ValidationMessage = validationMessage;
            AffectedFormIds = new List<string>(affectedFormIds);
        }

        public bool IsValid()
        {
            return _rule();
        }

        public bool IsInvalid()
        {
            return !_rule();
        }
    }
}
