using System;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SubmitButtonLabelAttribute : Attribute
    {
        public string Label { get; private set; }

        public SubmitButtonLabelAttribute(string label)
        {
            Label = label;
        }
    }
}
