using System.Collections.Generic;
using System.Linq;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public class DynamicFormWizardsFacade
    {
        public static IEnumerable<DynamicFormWizard> GetWizards()
        {
            return DefinitionsFacade.GetDefinitions().OfType<DynamicFormWizard>();
        }

        public static DynamicFormWizard GetWizard(string wizardName)
        {
            return (DynamicFormWizard)DefinitionsFacade.GetDefinition(wizardName);
        }

        public static void SaveWizard(DynamicFormWizard wizard)
        {
            var serializer = new WizardXmlSerializer();

            serializer.Save(wizard);
        }
    }
}
