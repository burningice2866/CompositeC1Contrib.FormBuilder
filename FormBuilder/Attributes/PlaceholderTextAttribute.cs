using System;

using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PlaceholderTextAttribute : Attribute
    {
        private string _text;
        public string Text
        {
            get { return FormRenderer.GetLocalized(_text); }
        }

        public PlaceholderTextAttribute(string text)
        {
            _text = text;
        }
    }
}
