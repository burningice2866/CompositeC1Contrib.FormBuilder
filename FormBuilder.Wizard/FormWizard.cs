using System.Collections.Generic;
using System.Xml.Linq;

namespace CompositeC1Contrib.FormBuilder.Wizard
{
    public class FormWizard
    {
        public string Name { get; set; }
        public IList<FormWizardStep> Steps { get; private set; }

        public FormWizard()
        {
            Steps = new List<FormWizardStep>();
        }

        public static FormWizard Parse(string name, XElement xml)
        {
            var wizard = new FormWizard { Name = name };

            var stepsElement = xml.Element("Steps");
            foreach (var stepElement in stepsElement.Elements("Step"))
            {
                var wizardStep = new FormWizardStep()
                {
                    Name = stepElement.Attribute("Name").Value,
                    FormName = stepElement.Attribute("FormName").Value,
                    LocalOrdering = int.Parse(stepElement.Attribute("LocalOrdering").Value),
                };

                wizard.Steps.Add(wizardStep);
            }

            return wizard;
        }
    }
}
