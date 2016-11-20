using System;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PlaceholderTextAttribute : Attribute
    {
        public string Text { get; private set; }

        public PlaceholderTextAttribute() { }

        public PlaceholderTextAttribute(string text)
        {
            Text = text;
        }
    }
}
