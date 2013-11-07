using System.Collections.Generic;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public class DynamicFormModelsProvider : IFormModelsProvider
    {
        public IEnumerable<FormModel> GetModels()
        {
            var definitions = DynamicFormsFacade.GetFormDefinitions();
            foreach (var def in definitions)
            {
                yield return def.Model;
            }
        }
    }
}
