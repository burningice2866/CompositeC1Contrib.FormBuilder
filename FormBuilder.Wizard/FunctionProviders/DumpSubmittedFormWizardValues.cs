using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Composite.C1Console.Security;
using Composite.Core.Xml;
using Composite.Functions;

using CompositeC1Contrib.FormBuilder.FunctionProviders;
using CompositeC1Contrib.FormBuilder.Wizard.Web;

namespace CompositeC1Contrib.FormBuilder.Wizard.FunctionProviders
{
    public class DumpSubmittedFormWizardValues : IFunction
    {
        public string Namespace { get; private set; }
        public string Name { get; private set; }

        public EntityToken EntityToken
        {
            get { return new FormBuilderFunctionEntityToken(typeof(FormBuilderFunctionProvider).Name, Name); }
        }

        public string Description
        {
            get { return String.Empty; }
        }

        public Type ReturnType
        {
            get { return typeof(XhtmlDocument); }
        }

        public IEnumerable<ParameterProfile> ParameterProfiles
        {
            get { return Enumerable.Empty<ParameterProfile>(); }
        }

        public DumpSubmittedFormWizardValues()
        {
            Namespace = "FormBuilder";
            Name = "DumpSubmittedFormWizardValues";
        }

        public object Execute(ParameterList parameters, FunctionContextContainer context)
        {
            var doc = new XhtmlDocument();
            var renderingContext = (FormWizardRequestContext)context.GetParameterValue(FormWizardFunction.RenderingContextKey, typeof(FormWizardRequestContext));

            foreach (var step in renderingContext.Wizard.Steps)
            {
                var model = renderingContext.StepModels[step.Name];

                doc.Body.Add(XElement.Parse("<h3>" + step.Name + "</h3>"));

                DumpSubmittedFormValues.DumpModelValues(model, doc);
            }

            return doc;
        }
    }
}
