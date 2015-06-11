using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;

namespace CompositeC1Contrib.FormBuilder
{
    public static class FormModelsFacade
    {
        public static readonly string RootPath = HostingEnvironment.MapPath("~/App_Data/FormBuilder");

        public static IEnumerable<IFormModelsProvider> Providers { get; private set; }

        static FormModelsFacade()
        {
            Providers = CompositionContainerFacade.GetExportedValues<IFormModelsProvider>().ToList();
        }

        public static IFormModel GetModel(string name)
        {
            return GetModels().Select(c => c.Model).SingleOrDefault(m => m.Name == name);
        }

        public static IEnumerable<ProviderModelContainer> GetModels()
        {
            return Providers.SelectMany(provider => provider.GetModels());
        }
    }
}