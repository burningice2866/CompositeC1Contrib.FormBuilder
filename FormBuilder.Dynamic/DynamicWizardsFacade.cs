using System.Collections.Generic;
using System.Linq;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public class DynamicWizardsFacade
    {
        public static IEnumerable<DynamicWizardDefinition> GetWizards()
        {
            return DefinitionsFacade.GetDefinitions().OfType<DynamicWizardDefinition>();
        }

        public static DynamicWizardDefinition GetWizard(string wizardName)
        {
            return (DynamicWizardDefinition)DefinitionsFacade.GetDefinition(wizardName);
        }

        public static void SaveWizard(DynamicWizardDefinition wizard)
        {
            var serializer = new WizardXmlSerializer();

            serializer.Save(wizard);
        }
    }
}
