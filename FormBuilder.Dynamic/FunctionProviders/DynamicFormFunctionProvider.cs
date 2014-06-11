using System;
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
                var definitions = DefinitionsFacade.GetDefinitions();
                foreach (var def in definitions)
                {
                    IFunction function;
                    if (FunctionFacade.TryGetFunction(out function, def.Name))
                    {
                        continue;
                    }

                    if (def is DynamicFormDefinition)
                    {
                        function = new StandardFormFunction<DynamicFormBuilderRequestContext>(def.Name, def.IntroText, def.SuccessResponse);
                    }

                    if (def is DynamicFormWizard)
                    {
                        function = new FormWizardFunction<DynamicFormWizardRequestContext>(def.Name, def.IntroText, def.SuccessResponse);
                    }

                    if (function != null)
                    {
                        if (def.Settings != null)
                        {
                            var executor = def.Settings.GetFormExecutor(def);
                            if (!String.IsNullOrEmpty(executor))
                            {
                                ((BaseFormFunction)function).OverrideFormExecutor = executor;
                            }
                        }

                        yield return function;
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
