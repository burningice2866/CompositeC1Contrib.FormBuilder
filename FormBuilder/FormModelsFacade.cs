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

        public static IEnumerable<IFormModelsProvider> Providers { get; private set; }

        static FormModelsFacade()
        {
            var batch = new CompositionBatch();
            var catalog = new SafeDirectoryCatalog(HttpRuntime.BinDirectory);
            var container = new CompositionContainer(catalog);

            container.Compose(batch);

            Providers = container.GetExportedValues<IFormModelsProvider>().ToList();
        }

        public static IFormModel GetModel(string name)
        {
            return GetModels().SingleOrDefault(m => m.Name == name);
        }

        public static IEnumerable<IFormModel> GetModels()
        {
            return Providers.SelectMany(provider => provider.GetModels());
        }
    }
}