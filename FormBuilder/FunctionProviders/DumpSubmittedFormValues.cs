using Composite.Core.Xml;
using Composite.Functions;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class DumpSubmittedFormValues : BaseDumpSubmittedFormValuesFunction
    {
        public DumpSubmittedFormValues()
        {
            Name = "DumpSubmittedFormValues";
        }

        public override object Execute(ParameterList parameters, FunctionContextContainer context)
        {
            var doc = new XhtmlDocument();
            var formModel = (FormModel)context.GetParameterValue(BaseFormFunction.FormModelKey, typeof(FormModel));

            DumpModelValues(formModel, doc);

            return doc;
        }
    }
}
