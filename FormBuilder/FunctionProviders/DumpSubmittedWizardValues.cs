using System.Xml.Linq;

using Composite.Core.Xml;
using Composite.Functions;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class DumpSubmittedWizardValues : BaseDumpSubmittedFormValuesFunction
    {
        public DumpSubmittedWizardValues()
        {
            Name = "DumpSubmittedWizardValues";
        }

        public override object Execute(ParameterList parameters, FunctionContextContainer context)
        {
            var doc = new XhtmlDocument();
            var useRenderingLayout = parameters.GetParameter<bool>("UseRenderingLayout");
            var wizard = (FormWizard)context.GetParameterValue(BaseFormFunction.FormModelKey, typeof(FormWizard));
            
            foreach (var step in wizard.Steps)
            {
                if (!useRenderingLayout)
                {
                    doc.Body.Add(XElement.Parse("<h3>" + step.Name + "</h3>"));
                }

                DumpModelValues(step.FormModel, doc, useRenderingLayout);
            }

            return doc;
        }
    }
}
