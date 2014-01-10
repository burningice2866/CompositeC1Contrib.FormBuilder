using System;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    [Obsolete("Use RenderingLayout instead")]
    public class TextBeforeAttribute : Attribute
    {
        public string Text { get; private set; }

        public TextBeforeAttribute(string text)
        {
            Text = text;
        }
    }
}
