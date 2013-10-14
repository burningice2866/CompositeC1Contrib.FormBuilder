using System;

namespace CompositeC1Contrib.FormBuilder
{
    public class FormValidationEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }
}
