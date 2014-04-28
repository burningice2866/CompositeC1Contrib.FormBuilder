using System.Xml.Linq;

using Composite.Core.Xml;
using Composite.Functions;

using CompositeC1Contrib.FormBuilder.Wizard;

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
            var wizard = (FormWizard)context.GetParameterValue(BaseFormFunction.FormModelKey, typeof(FormWizard));

            foreach (var step in wizard.Steps)
            {
                var model = wizard.StepModels[step.Name];

                doc.Body.Add(XElement.Parse("<h3>" + step.Name + "</h3>"));

                DumpModelValues(model, doc);
            }

            return doc;
        }
    }
}
