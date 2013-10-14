using System;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class HtmlTagAttribute : Attribute
    {
        public string Attribute { get; private set; }
        public string Value { get; private set; }

        public HtmlTagAttribute(string attribute, string value)
        {
            Attribute = attribute;
            Value = value;
        }
    }
}
