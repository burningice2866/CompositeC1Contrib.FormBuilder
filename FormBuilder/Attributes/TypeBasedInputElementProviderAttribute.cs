using System;

using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TypeBasedInputElementProviderAttribute : InputElementProviderAttribute
    {
        private Type _type;

        public TypeBasedInputElementProviderAttribute(Type type)
        {
            _type = type;
        }

        public override IInputElementHandler GetInputFieldTypeHandler()
        {
            return (IInputElementHandler)Activator.CreateInstance(_type);
        }
    }
}
