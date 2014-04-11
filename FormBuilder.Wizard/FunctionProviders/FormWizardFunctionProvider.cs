using System.Collections.Generic;

using Composite.Functions;
using Composite.Functions.Plugins.FunctionProvider;

using CompositeC1Contrib.FormBuilder.Wizard;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class FormWizardFunctionProvider : IFunctionProvider
    {
        public FunctionNotifier FunctionNotifier { private get; set; }

        public IEnumerable<IFunction> Functions
        {
            get
            {
                var wizards = FormWizardsFacade.GetWizards();
                foreach (var wizard in wizards)
                {
                    IFunction function;
                    if (!FunctionFacade.TryGetFunction(out function, wizard.Name))
                    {
                        yield return new FormWizardFunction(wizard.Name);
                    }
                }
            }
        }

        public FormWizardFunctionProvider()
        {
            FormWizardsFacade.SubscribeToFormWizardChanges(() =>
            {
                if (FunctionNotifier != null)
                {
                    FunctionNotifier.FunctionsUpdated();
                }
            });
        }
    }
}
