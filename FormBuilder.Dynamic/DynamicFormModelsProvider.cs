using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    [Export(typeof(IFormModelsProvider))]
    public class DynamicFormModelsProvider : IFormModelsProvider
    {
        public IEnumerable<FormModel> GetModels()
        {
            var definitions = DynamicFormsFacade.GetFormDefinitions();
            
            return definitions.Select(def => def.Model);
        }
    }
}
