using System;
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
            var instance = (Wizard)context.GetParameterValue(BaseFormFunction.InstanceKey, typeof(Wizard));

            foreach (var step in instance.Steps)
            {
                if (!useRenderingLayout)
                {
                    doc.Body.Add(XElement.Parse("<h3>" + (String.IsNullOrEmpty(step.Label) ? step.Name : step.Label) + "</h3>"));
                }

                DumpModelValues(step.Form, doc, useRenderingLayout);
            }

            return doc;
        }
    }
}
