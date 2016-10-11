using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using CompositeC1Contrib.FormBuilder.FunctionProviders;
using CompositeC1Contrib.FormBuilder.Web;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    [Export(typeof(IModelsProvider))]
    public class DynamicModelsProvider : IModelsProvider
    {
        public IEnumerable<ProviderModelContainer> GetModels()
        {
            var definitions = DefinitionsFacade.GetDefinitions();
            foreach (var def in definitions)
            {
                string executor = null;

                if (def.Settings != null)
                {
                    executor = def.Settings.GetFormExecutor(def);
                }

                var form = def as DynamicFormDefinition;
                if (form != null)
                {
                    var functionName = GetFunctionName(def.Name);
                    var function = new StandardFormFunction<DynamicFormBuilderRequestContext>(functionName, def.IntroText, def.SuccessResponse);

                    if (!String.IsNullOrEmpty(executor))
                    {
                        function.OverrideFormExecutor = executor;
                    }

                    yield return new ProviderModelContainer
                    {
                        Source = typeof(DynamicModelsProvider),
                        Type = "DynamicForm",
                        Model = form.Model,
                        Function = function
                    };
                }

                var wizard = def as DynamicWizardDefinition;
                if (wizard != null)
                {
                    var functionName = GetFunctionName(def.Name);
                    var function = new FormWizardFunction<DynamicFormWizardRequestContext>(functionName, def.IntroText, def.SuccessResponse);

                    if (!String.IsNullOrEmpty(executor))
                    {
                        function.OverrideFormExecutor = executor;
                    }

                    yield return new ProviderModelContainer
                    {
                        Source = typeof(DynamicModelsProvider),
                        Type = "DynamicWizard",
                        Model = wizard.Model,
                        Function = function
                    };
                }
            }
        }

        private string GetFunctionName(string name)
        {
            if (!name.StartsWith("Forms."))
            {
                name = "Forms." + name;
            }

            return name;
        }
    }
}
