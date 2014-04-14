using System.Collections.Generic;

using Composite.Functions;
using Composite.Functions.Plugins.FunctionProvider;

using CompositeC1Contrib.FormBuilder.Web;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class POCOFormFunctionProvider : IFunctionProvider
    {
        public FunctionNotifier FunctionNotifier { private get; set; }

        public IEnumerable<IFunction> Functions
        {
            get
            {
                var models = POCOFormModelsProvider.GetFormsAndModels();
                foreach (var entry in models)
                {
                    IFunction function;
                    if (!FunctionFacade.TryGetFunction(out function, entry.Value.Name))
                    {
                        yield return new StandardFormFunction<POCOFormBuilderRequestContext>(entry.Value.Name);
                    }
                }
            }
        }

        
    }
}
