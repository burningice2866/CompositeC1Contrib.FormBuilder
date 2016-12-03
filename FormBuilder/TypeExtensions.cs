using System;

namespace CompositeC1Contrib.FormBuilder
{
    public static class TypeExtensions
    {
        public static string NameWithoutTrailingAttribute(this Type type)
        {
            var name = type.Name;

            return name.EndsWith("Attribute") ? name.Substring(0, name.Length - "Attribute".Length) : name;
        }
    }
}
