using System;

using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FunctionBasedInputElementProviderAttribute : InputElementProviderAttribute
    {
        private string _functionName;

        public FunctionBasedInputElementProviderAttribute(string functionName)
        {
            _functionName = functionName;
        }

        public override IInputElementHandler GetInputFieldTypeHandler()
        {
            return new FunctionbasedInputElement(_functionName);
        }
    }
}
