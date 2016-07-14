using System;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PlaceholderTextAttribute : Attribute
    {
        private string _text;
        public string Text
        {
            get { return _text == null ? null : Strings.GetLocalized(_text); }
        }

        public PlaceholderTextAttribute() { }

        public PlaceholderTextAttribute(string text)
        {
            _text = text;
        }
    }
}
