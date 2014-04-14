using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Composite.Core.Xml;

using CompositeC1Contrib.FormBuilder.Wizard.SubmitHandlers;

namespace CompositeC1Contrib.FormBuilder.Wizard
{
    public class FormWizard
    {
        public string Name { get; set; }
        public bool ForceHttpSConnection { get; set; }
        public XhtmlDocument IntroText { get; set; }
        public XhtmlDocument SuccessResponse { get; set; }
        public IList<FormWizardStep> Steps { get; private set; }
        public IList<FormSubmitHandler> SubmitHandlers { get; private set; }

        public FormWizard()
        {
            IntroText = new XhtmlDocument();
            SuccessResponse = new XhtmlDocument();
            Steps = new List<FormWizardStep>();
            SubmitHandlers = new List<FormSubmitHandler>();
        }

        public static FormWizard Parse(string name, XElement xml)
        {
            var wizard = new FormWizard { Name = name };

            var forceHttpsConnectionAttribute = xml.Attribute("forceHttpsConnection");
            if (forceHttpsConnectionAttribute != null)
            {
                wizard.ForceHttpSConnection = bool.Parse(forceHttpsConnectionAttribute.Value);
            }

            var stepsElement = xml.Element("Steps");
            foreach (var stepElement in stepsElement.Elements("Step"))
            {
                var wizardStep = new FormWizardStep()
                {
                    Name = stepElement.Attribute("Name").Value,
                    FormName = stepElement.Attribute("FormName").Value,
                    LocalOrdering = int.Parse(stepElement.Attribute("LocalOrdering").Value),
                };

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
                    var handlerName = handler.Attribute("Name").Value;
                    var typeString = handler.Attribute("Type").Value;
                    var type = Type.GetType(typeString);
                    var instance = (FormSubmitHandler)XElementHelper.DeserializeInstanceWithArgument(type, handler);

                    instance.Name = handlerName;

                    wizard.SubmitHandlers.Add(instance);
                }
            }

            return wizard;
        }
    }
}
