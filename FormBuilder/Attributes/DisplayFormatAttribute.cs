using System;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DisplayFormatAttribute : Attribute
    {
        public virtual string FormatString { get; protected set; }

        public DisplayFormatAttribute(string formatString)
        {
            FormatString = formatString;
        }
    }
}
