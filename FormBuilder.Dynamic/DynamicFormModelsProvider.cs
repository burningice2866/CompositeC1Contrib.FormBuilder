using System.Collections.Generic;
using System.Linq;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public class DynamicFormModelsProvider : IFormModelsProvider
    {
        public IEnumerable<FormModel> GetModels()
        {
            var definitions = DynamicFormsFacade.GetFormDefinitions();
            
            return definitions.Select(def => def.Model);
        }
    }
}
