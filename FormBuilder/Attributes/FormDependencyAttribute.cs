using System;

using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public abstract class FormDependencyAttribute: Attribute
    {
        public string ReadFromFieldName { get; private set; }

        protected FormDependencyAttribute(string readFromFieldName)
        {
            this.ReadFromFieldName = readFromFieldName;
        }

        public abstract bool DependencyMet(FormModel model);
        public abstract string[] RequiredFieldValues();
    }
}
