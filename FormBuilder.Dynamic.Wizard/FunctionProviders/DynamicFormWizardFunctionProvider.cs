using System.Collections.Generic;

using Composite.Functions;
using Composite.Functions.Plugins.FunctionProvider;

using CompositeC1Contrib.FormBuilder.Dynamic.Wizard;
using CompositeC1Contrib.FormBuilder.Web;
using CompositeC1Contrib.FormBuilder.Wizard;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class DynamicFormWizardFunctionProvider : IFunctionProvider
    {
        public FunctionNotifier FunctionNotifier { private get; set; }

        public IEnumerable<IFunction> Functions
        {
            get
            {
                var wizards = DynamicFormWizardsFacade.GetWizards();
                foreach (var wizard in wizards)
                {
                    IFunction function;
                    if (!FunctionFacade.TryGetFunction(out function, wizard.Name))
                    {
                        yield return new FormWizardFunction<DynamicFormWizardRequestContext>(wizard.Name);
                    }
                }
            }
        }

        public DynamicFormWizardFunctionProvider()
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
