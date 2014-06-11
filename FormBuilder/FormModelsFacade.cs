using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace CompositeC1Contrib.FormBuilder
{
    public static class FormModelsFacade
    {
        public static readonly string RootPath = HostingEnvironment.MapPath("~/App_Data/FormBuilder");

        private static readonly IList<IFormModelsProvider> ModelProviders;

        static FormModelsFacade()
        {
            var batch = new CompositionBatch();
            var catalog = new SafeDirectoryCatalog(HttpRuntime.BinDirectory);
            var container = new CompositionContainer(catalog);

            container.Compose(batch);

            ModelProviders = container.GetExportedValues<IFormModelsProvider>().ToList();
        }

        public static IFormModel GetModel(string name)
        {
            return GetModels().SingleOrDefault(m => m.Name == name);
        }

        public static IEnumerable<IFormModel> GetModels()
        {
            return ModelProviders.SelectMany(provider => provider.GetModels());
        }
    }
}