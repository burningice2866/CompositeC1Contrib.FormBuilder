using System;

namespace CompositeC1Contrib.FormBuilder
{
    public static class AttributeExtensions
    {
        public static string AttributeName(this Attribute attribute)
        {
            return attribute.GetType().NameWithoutTrailingAttribute();
        }
    }
}
