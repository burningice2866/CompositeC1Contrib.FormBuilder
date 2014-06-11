using System;
using System.IO;
using System.Xml.Linq;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public class WizardXmlSerializer : XmlDefinitionSerializer
    {
        public override IDynamicFormDefinition Load(string name, XElement xml)
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
                ParseLayout(layoutElement, wizard);
            }

            ParseSubmitHandlers(xml, wizard);

            return wizard;
        }

        public override void Save(IDynamicFormDefinition form)
        {
            var wizard = (DynamicFormWizard)form;
            var dir = Path.Combine(FormModelsFacade.RootPath, wizard.Name);
            var file = Path.Combine(dir, "Definition.xml");

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var root = new XElement("FormBuilder.Wizard",
                new XAttribute("name", wizard.Name),
                new XAttribute("forceHttpsConnection", wizard.ForceHttpSConnection));

            var layout = new XElement("Layout");

            SaveLayout(form, layout);

            root.Add(layout);

            var steps = new XElement("Steps");

            foreach (var step in wizard.Steps)
            {
                var stepElement = new XElement("Step",
                    new XAttribute("Name", step.Name),
                    new XAttribute("FormName", step.FormName),
                    new XAttribute("LocalOrdering", step.LocalOrdering));

                if (!String.IsNullOrEmpty(step.NextButtonLabel))
                {
                    stepElement.Add(new XAttribute("nextButtonLabel", step.NextButtonLabel));
                }

                if (!String.IsNullOrEmpty(step.PreviousButtonLabel))
                {
                    stepElement.Add(new XAttribute("previousButtonLabel", step.PreviousButtonLabel));
                }

                steps.Add(stepElement);
            }

            root.Add(steps);

            SaveMetaData(form, root);

            root.Save(file);

            base.Save(form);
        }
    }
}
