using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

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

        public static void SaveWizard(FormWizard wizard)
        {
            var dir = Path.Combine(WizardsPath, wizard.Name);
            var file = Path.Combine(dir, "Definition.xml");

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var root = new XElement("FormBuilder.Wizard",
                new XAttribute("name", wizard.Name));

            var steps = new XElement("Steps");

            foreach (var step in wizard.Steps)
            {
                var stepElement = new XElement("Step",
                    new XAttribute("Name", step.Name),
                    new XAttribute("FormName", step.FormName),
                    new XAttribute("LocalOrdering", step.LocalOrdering));

                steps.Add(stepElement);
            }

            root.Add(steps);
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
