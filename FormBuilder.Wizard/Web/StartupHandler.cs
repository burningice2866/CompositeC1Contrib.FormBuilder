using System.IO;

using Composite.Core.Application;

namespace CompositeC1Contrib.FormBuilder.Wizard.Web
{
    [ApplicationStartup]
    public class StartupHandler
    {
        public static void OnBeforeInitialize()
        {
            if (!Directory.Exists(FormWizardsFacade.WizardsPath))
            {
                Directory.CreateDirectory(FormWizardsFacade.WizardsPath);
            }
        }

        public static void OnInitialized() { }
    }
}
