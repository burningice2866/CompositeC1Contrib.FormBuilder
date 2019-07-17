using System;
using System.Collections.Generic;

using Composite;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class FormValidationRule
    {
        private Func<bool> _rule;
        private string _validationMessage;

        public IEnumerable<string> AffectedFormIds { get; }
        public Func<string, string> FormatMessage { get; set; }

        public string ValidationMessage
        {
            get
            {
                if (FormatMessage != null)
                {
                    return FormatMessage(_validationMessage);
                }

                return _validationMessage;
            }
        }

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

            _validationMessage = validationMessage;
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
