using System.Collections.Generic;

namespace CompositeC1Contrib.FormBuilder
{
    public interface IFormModelsProvider
    {
        IEnumerable<ProviderModelContainer> GetModels();
    }
}
