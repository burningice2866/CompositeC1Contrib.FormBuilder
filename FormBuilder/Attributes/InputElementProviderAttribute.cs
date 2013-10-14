using System;

using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class InputElementProviderAttribute : Attribute
    {
        public InputElementProviderAttribute() { }

        public abstract IInputElementHandler GetInputFieldTypeHandler();
    }
}
