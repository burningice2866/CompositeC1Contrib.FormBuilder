using System;
using System.Collections.Generic;
using System.Xml.Linq;

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
                    IFunction function = null;
                    if (!FunctionFacade.TryGetFunction(out function, def.Name))
                    {
                        yield return new StandardFormFunction<DynamicFormBuilderRequestContext>(def.Name)
                        {
                            OverrideFormExecutor = def.FormExecutor
                        };
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
