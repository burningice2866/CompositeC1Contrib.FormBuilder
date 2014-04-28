using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using CompositeC1Contrib.FormBuilder.Wizard;

namespace CompositeC1Contrib.FormBuilder.Dynamic.Wizard
{
    public class DynamicFormWizardsFacade
    {
        public static IEnumerable<DynamicFormWizard> GetWizards()
        {
            var files = Directory.GetFiles(FormWizardsFacade.WizardsPath, "Definition.xml", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var folder = Path.GetDirectoryName(file);
                var name = new DirectoryInfo(folder).Name;
                var xml = XElement.Load(file);

                yield return DynamicFormWizard.Parse(name, xml);
            }
        }

        public static DynamicFormWizard GetWizard(string wizardName)
        {
            var file = Path.Combine(FormWizardsFacade.WizardsPath, wizardName, "Definition.xml");
            if (!File.Exists(file))
            {
                return null;
            }

            var xml = XElement.Load(file);

            return DynamicFormWizard.Parse(wizardName, xml);
        }

        public static void DeleteWizard(DynamicFormWizard wizard)
        {
            var dir = Path.Combine(FormWizardsFacade.WizardsPath, wizard.Name);

            Directory.Delete(dir, true);

            foreach (var submithandler in wizard.SubmitHandlers)
            {
                submithandler.Delete(wizard);
            }

            FormWizardsFacade.NotifyChanges();
        }

        public static void SaveWizard(DynamicFormWizard wizard)
        {
            var dir = Path.Combine(FormWizardsFacade.WizardsPath, wizard.Name);
            var file = Path.Combine(dir, "Definition.xml");

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var root = new XElement("FormBuilder.Wizard",
                new XAttribute("name", wizard.Name),
                new XAttribute("forceHttpsConnection", wizard.ForceHttpSConnection));

            var layout = new XElement("Layout");

            layout.Add(new XElement("introText", wizard.IntroText.ToString()));
            layout.Add(new XElement("successResponse", wizard.SuccessResponse.ToString()));

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

            if (wizard.SubmitHandlers.Any())
            {
                var submitHandlers = new XElement("SubmitHandlers");

                foreach (var handler in wizard.SubmitHandlers)
                {
                    var handlerElement = new XElement("Add",
                        new XAttribute("Name", handler.Name),
                        new XAttribute("Type", handler.GetType().AssemblyQualifiedName));

                    handler.Save(wizard, handlerElement);

                    submitHandlers.Add(handlerElement);
                }

                root.Add(submitHandlers);
            }

            root.Save(file);

            FormWizardsFacade.NotifyChanges();
        }
    }
}
