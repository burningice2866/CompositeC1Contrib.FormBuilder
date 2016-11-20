using System;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FieldHelpAttribute : Attribute
    {
        public string Help { get; private set; }

        public FieldHelpAttribute() { }

        public FieldHelpAttribute(string help)
        {
            Help = help;
        }
    }
}
