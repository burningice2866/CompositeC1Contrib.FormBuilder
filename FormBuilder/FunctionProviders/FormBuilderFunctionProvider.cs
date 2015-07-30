using System.Collections.Generic;

using Composite.Functions;
using Composite.Functions.Plugins.FunctionProvider;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class FormBuilderFunctionProvider : IFunctionProvider
    {
        public FunctionNotifier FunctionNotifier { private get; set; }

        public IEnumerable<IFunction> Functions
        {
            get
            {
                yield return new DumpSubmittedFormValues();
                yield return new DumpSubmittedWizardValues();

                foreach (var entry in ModelsFacade.GetModels())
                {
                    IFunction function;
                    if (!FunctionFacade.TryGetFunction(out function, entry.Model.Name))
                    {
                        yield return entry.Function;
                    }
                }
            }
        }

        public FormBuilderFunctionProvider()
        {
            ModelsFacade.FormChanges += (sender, args) =>
            {
                if (FunctionNotifier != null)
                {
                    FunctionNotifier.FunctionsUpdated();
                }
            };
        }
    }
}
