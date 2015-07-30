using System;

namespace CompositeC1Contrib.FormBuilder
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ExcludeFieldAttribute : Attribute { }
}
