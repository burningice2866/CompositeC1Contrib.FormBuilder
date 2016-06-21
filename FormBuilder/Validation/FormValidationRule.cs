using System;
using System.Collections.Generic;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class FormValidationRule
    {
        public IEnumerable<string> AffectedFormIds { get; private set; }
        public Func<bool> Rule { get; set; }

        private readonly string _validationMessage;
        public string ValidationMessage
        {
            get { return Strings.GetLocalized(_validationMessage); }
        }

        public FormValidationRule(IEnumerable<string> affectedFormIds, string validationMessage)
        {
            _validationMessage = validationMessage;
            AffectedFormIds = new List<string>(affectedFormIds);            
        }
    }
}
