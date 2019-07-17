using System;
using System.Collections.Specialized;

namespace CompositeC1Contrib.FormBuilder
{
    public class FormValidationEventArgs : EventArgs
    {
        public NameValueCollection SubmittedValues { get; }
        public bool Cancel { get; set; }

        public FormValidationEventArgs(NameValueCollection submittedValues)
        {
            SubmittedValues = submittedValues;
        }
    }
}
