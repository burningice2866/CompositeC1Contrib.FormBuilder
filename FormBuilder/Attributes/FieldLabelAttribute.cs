using System;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FieldLabelAttribute : Attribute
    {
        public string Label { get; }

        public FieldLabelAttribute() { }

        public FieldLabelAttribute(string label)
        {
            Label = label;
        }
    }
}
