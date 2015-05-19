using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    [Export(typeof(IFormModelsProvider))]
    public class DynamicFormModelsProvider : IFormModelsProvider
    {
        public IEnumerable<ProviderModelContainer> GetModels()
        {
            var definitions = DefinitionsFacade.GetDefinitions();
            foreach (var def in definitions)
            {
                var form = def as DynamicFormDefinition;
                if (form != null)
                {
                    yield return new ProviderModelContainer
                    {
                        Source = typeof(DynamicFormModelsProvider),
                        Type = "DynamicForm",
                        Model = form.Model
                    };
                }

                var wizard = def as DynamicFormWizard;
                if (wizard != null)
                {
                    yield return new ProviderModelContainer
                    {
                        Source = typeof(DynamicFormModelsProvider),
                        Type = "DynamicWizard",
                        Model = wizard
                    };
                }
            }
        }
    }
}
