using System;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FieldHelpAttribute : Attribute
    {
        private string _help;
        public string Help
        {
            get { return _help == null ? null : Strings.GetLocalized(_help); }
        }

        public FieldHelpAttribute() { }

        public FieldHelpAttribute(string help)
        {
            _help = help;
        }
    }
}
