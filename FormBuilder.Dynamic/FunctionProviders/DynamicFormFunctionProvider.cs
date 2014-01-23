using System.Collections.Generic;

using Composite.Functions;
using Composite.Functions.Plugins.FunctionProvider;

using CompositeC1Contrib.FormBuilder.Dynamic;

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
                        function = new StandardFormFunction<DynamicFormBuilderRequestContext>(def.Name, def.IntroText, def.SuccessResponse)
                        {
                            OverrideFormExecutor = def.FormExecutor
                        };

                        yield return function;
                    }
                }
            }
        }

        public DynamicFormFunctionProvider()
        {
            DynamicFormsFacade.SubscribeToFormChanges(() =>
            {
                if (FunctionNotifier != null)
                {
                    FunctionNotifier.FunctionsUpdated();
                }
            });
        }
    }
}
