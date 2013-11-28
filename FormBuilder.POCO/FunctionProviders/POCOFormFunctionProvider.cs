using System.Collections.Generic;

using Composite.Functions;
using Composite.Functions.Plugins.FunctionProvider;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class POCOFormFunctionProvider : IFunctionProvider
    {
        public FunctionNotifier FunctionNotifier { private get; set; }

        public IEnumerable<IFunction> Functions
        {
            get
            {
                var models = POCOFormModelsProvider.GetModels();
                foreach (var entry in models)
                {
                    IFunction function = null;
                    if (!FunctionFacade.TryGetFunction(out function, entry.Value.Name))
                    {
                        var instance = entry.Key;

                        yield return new StandardFormFunction(entry.Value)
                        {
                            OnSubmit = instance.Submit,
                            OnMappedValues = (m) => POCOFormsFacade.SetDefaultValues(instance, m),
                            SetDefaultValues = (m) => POCOFormsFacade.SetDefaultValues(instance, m)
                        };
                    }
                }
            }
        }

        
    }
}
