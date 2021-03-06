﻿using System;
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

                if (def is DynamicFormDefinition form)
                {
                    var function = new StandardFormFunction<DynamicFormBuilderRequestContext>(form.Model);

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

                if (def is DynamicWizardDefinition wizard)
                {
                    var function = new FormWizardFunction<DynamicFormWizardRequestContext>(wizard.Model);

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
    }
}
