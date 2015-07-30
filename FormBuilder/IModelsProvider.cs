using System.Collections.Generic;

namespace CompositeC1Contrib.FormBuilder
{
    public interface IModelsProvider
    {
        IEnumerable<ProviderModelContainer> GetModels();
    }
}
