using System.Collections.Generic;

using Composite.Functions;
using Composite.Functions.Plugins.FunctionProvider;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class FormWizardFunctionProvider : IFunctionProvider
    {
        public FunctionNotifier FunctionNotifier { private get; set; }

        public IEnumerable<IFunction> Functions
        {
            get
            {
                yield return new DumpSubmittedWizardValues();
            }
        }
    }
}
