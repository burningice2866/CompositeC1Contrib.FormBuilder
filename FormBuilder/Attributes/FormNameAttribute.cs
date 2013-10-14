using System;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FormNameAttribute : Attribute
    {
        public string Namespace { get; private set; }
        public string Name { get; private set; }

        public string FullName
        {
            get { return Namespace + "." + Name; }
        }

        public FormNameAttribute(string ns, string name)
        {
            Namespace = ns;
            Name = name;
        }
    }
}
