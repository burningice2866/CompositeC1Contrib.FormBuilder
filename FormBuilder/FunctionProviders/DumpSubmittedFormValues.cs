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
            var useRenderingLayout = parameters.GetParameter<bool>("UseRenderingLayout");
            var instance = (Form)context.GetParameterValue(BaseFormFunction.InstanceKey, typeof(Form));

            DumpModelValues(instance, doc, useRenderingLayout);

            return doc;
        }
    }
}
