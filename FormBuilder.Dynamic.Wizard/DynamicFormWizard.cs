using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Composite.Core.Xml;

using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;
using CompositeC1Contrib.FormBuilder.Wizard;

namespace CompositeC1Contrib.FormBuilder.Dynamic.Wizard
{
    public class DynamicFormWizard : FormWizard, IDynamicFormDefinition
    {
        public IList<FormSubmitHandler> SubmitHandlers { get; private set; }

        public DynamicFormWizard()
        {
            SubmitHandlers = new List<FormSubmitHandler>();
        }

        public static DynamicFormWizard Parse(string name, XElement xml)
        {
            var wizard = new DynamicFormWizard { Name = name };

            var forceHttpsConnectionAttribute = xml.Attribute("forceHttpsConnection");
            if (forceHttpsConnectionAttribute != null)
            {
                wizard.ForceHttpSConnection = bool.Parse(forceHttpsConnectionAttribute.Value);
            }

            var stepsElement = xml.Element("Steps");
            foreach (var stepElement in stepsElement.Elements("Step"))
            {
                var wizardStep = new FormWizardStep
                {
                    Name = stepElement.Attribute("Name").Value,
                    FormName = stepElement.Attribute("FormName").Value,
                    LocalOrdering = int.Parse(stepElement.Attribute("LocalOrdering").Value),
                };

                var model = FormModelsFacade.GetModel(wizardStep.FormName);

                var nextButtonLabelAttribute = stepElement.Attribute("nextButtonLabel");
                if (nextButtonLabelAttribute != null)
                {
                    wizardStep.NextButtonLabel = nextButtonLabelAttribute.Value;
                }

                var previousButtonLabelAttribute = stepElement.Attribute("previousButtonLabel");
                if (previousButtonLabelAttribute != null)
                {
                    wizardStep.PreviousButtonLabel = previousButtonLabelAttribute.Value;
                }

                wizard.Steps.Add(wizardStep);
                wizard.StepModels.Add(wizardStep.Name, model);
            }

            var layoutElement = xml.Element("Layout");
            if (layoutElement != null)
            {
                var introText = layoutElement.Element("introText");
                if (introText != null)
                {
                    wizard.IntroText = XhtmlDocument.Parse(introText.Value);
                }

                var successResponse = layoutElement.Element("successResponse");
                if (successResponse != null)
                {
                    wizard.SuccessResponse = XhtmlDocument.Parse(successResponse.Value);
                }
            }

            var submitHandlersElement = xml.Element("SubmitHandlers");
            if (submitHandlersElement != null)
            {
                foreach (var handler in submitHandlersElement.Elements("Add"))
                {
                    var typeString = handler.Attribute("Type").Value;
                    var type = Type.GetType(typeString);
                    var instance = (FormSubmitHandler)XElementHelper.DeserializeInstanceWithArgument(type, handler);

                    instance.Load(wizard, handler);

                    wizard.SubmitHandlers.Add(instance);
                }
            }

            return wizard;
        }

        public override void Submit()
        {
            foreach (var handler in SubmitHandlers)
            {
                handler.Submit(this);
            }

            base.Submit();
        }
    }
}
