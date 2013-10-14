using System;

using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FieldLabelAttribute : Attribute
    {
        private string _label;
        public string Label
        {
            get { return FormRenderer.GetLocalized(_label); }
        }

        public string Link { get; set; }
        public bool OpenLinkInNewWindow { get; set; }

        public FieldLabelAttribute(string label)
        {
            _label = label;

            OpenLinkInNewWindow = false;
        }
    }
}
