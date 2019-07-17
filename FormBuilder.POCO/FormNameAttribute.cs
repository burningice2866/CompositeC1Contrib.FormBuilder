using System;

namespace CompositeC1Contrib.FormBuilder
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class FormNameAttribute : Attribute
    {
        public string Namespace { get; }
        public string Name { get; }

        public string FullName => Namespace + "." + Name;

        public FormNameAttribute(string ns, string name)
        {
            Namespace = ns;
            Name = name;
        }
    }
}
