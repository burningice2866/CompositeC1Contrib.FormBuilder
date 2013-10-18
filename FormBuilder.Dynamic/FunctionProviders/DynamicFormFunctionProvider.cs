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
                var forms = DynamicFormsFacade.GetFormDefinitions();

                foreach (var form in forms)
                {
                    yield return new DynamicFormFunction(form);
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
