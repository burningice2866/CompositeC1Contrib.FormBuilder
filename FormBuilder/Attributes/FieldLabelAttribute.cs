using System;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FieldLabelAttribute : Attribute
    {
        private readonly string _label;
        public string Label
        {
            get { return _label == null ? null : Strings.GetLocalized(_label); }
        }

        public FieldLabelAttribute() : this(null) { }

        public FieldLabelAttribute(string label)
        {
            _label = label;
        }
    }
}
