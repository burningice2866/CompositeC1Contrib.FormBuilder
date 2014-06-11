using System.Collections.Generic;

using Composite.Functions;
using Composite.Functions.Plugins.FunctionProvider;

using CompositeC1Contrib.FormBuilder.Dynamic;
using CompositeC1Contrib.FormBuilder.Web;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class DynamicFormFunctionProvider : IFunctionProvider
    {
        public FunctionNotifier FunctionNotifier { private get; set; }

        public IEnumerable<IFunction> Functions
        {
            get
            {
                var definitions = DynamicFormsFacade.GetFormDefinitions();
                foreach (var def in definitions)
                {
                    IFunction function;
                    if (!FunctionFacade.TryGetFunction(out function, def.Name))
                    {
                        function = new StandardFormFunction<DynamicFormBuilderRequestContext>(def.Name, def.IntroText, def.SuccessResponse);

                        yield return function;
                    }
                }

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

        public DynamicFormFunctionProvider()
        {
            DefinitionsFacade.SubscribeToFormChanges(() =>
            {
                if (FunctionNotifier != null)
                {
                    FunctionNotifier.FunctionsUpdated();
                }
            });
        }
    }
}
