using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using CompositeC1Contrib.FormBuilder.Wizard.SubmitHandlers;

namespace CompositeC1Contrib.FormBuilder.Wizard
{
    public class FormWizardsFacade
    {
        private static readonly IList<Action> FormWizardChangeNotifications = new List<Action>();

        public static readonly string WizardsPath = Path.Combine(FormModelsFacade.RootPath, "Wizards");

        public static void SubscribeToFormWizardChanges(Action notify)
        {
            FormWizardChangeNotifications.Add(notify);
        }

        public static IEnumerable<FormWizard> GetWizards()
        {
            var files = Directory.GetFiles(WizardsPath, "Definition.xml", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var folder = Path.GetDirectoryName(file);
                var name = new DirectoryInfo(folder).Name;
                var xml = XElement.Load(file);

                yield return FormWizard.Parse(name, xml);
            }
        }

        public static FormWizard GetWizard(string wizardName)
        {
            var file = Path.Combine(WizardsPath, wizardName, "Definition.xml");
            if (!File.Exists(file))
            {
                return null;
            }

            var xml = XElement.Load(file);

            return FormWizard.Parse(wizardName, xml);
        }

        public static void DeleteWizard(FormWizard wizard)
        {
            var dir = Path.Combine(WizardsPath, wizard.Name);

            Directory.Delete(dir, true);

            NotifyChanges();
        }

        public static void SaveWizard(FormWizard wizard)
        {
            var dir = Path.Combine(WizardsPath, wizard.Name);
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

                    var emailSubmitHandler = handler as EmailSubmitHandler;
                    if (emailSubmitHandler != null)
                    {
                        handlerElement.Add(new XAttribute("IncludeAttachments", emailSubmitHandler.IncludeAttachments));
                        handlerElement.Add(new XAttribute("From", emailSubmitHandler.From ?? String.Empty));
                        handlerElement.Add(new XAttribute("To", emailSubmitHandler.To ?? String.Empty));
                        handlerElement.Add(new XAttribute("Cc", emailSubmitHandler.Cc ?? String.Empty));
                        handlerElement.Add(new XAttribute("Bcc", emailSubmitHandler.Bcc ?? String.Empty));

                        handlerElement.Add(new XElement("Subject", emailSubmitHandler.Subject ?? String.Empty));
                        handlerElement.Add(new XElement("Body", emailSubmitHandler.Body ?? String.Empty));
                    }

                    submitHandlers.Add(handlerElement);
                }

                root.Add(submitHandlers);
            }

            root.Save(file);

            NotifyChanges();
        }

        private static void NotifyChanges()
        {
            foreach (var action in FormWizardChangeNotifications)
            {
                action();
            }
        }
    }
}
