using System;
using System.Collections.Generic;

using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    public class FormValidationRule
    {
        public IEnumerable<string> AffectedFormIds { get; private set; }
        public Func<bool> Rule { get; set; }

        private string _validationMessage;
        public string ValidationMessage
        {
            get { return FormRenderer.GetLocalized(_validationMessage); }
        }

        public FormValidationRule(string[] affectedFormIds, string validationMessage)
        {
            _validationMessage = validationMessage;
            AffectedFormIds = new List<string>(affectedFormIds);            
        }
    }
}
