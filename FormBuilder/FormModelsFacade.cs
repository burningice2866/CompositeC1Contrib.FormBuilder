using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace CompositeC1Contrib.FormBuilder
{
    public static class FormModelsFacade
    {
        public static readonly string RootPath = HostingEnvironment.MapPath("~/App_Data/FormBuilder");
        public static readonly string FormsPath = Path.Combine(RootPath, "Forms");

        private static readonly IList<IFormModelsProvider> ModelProviders;

        static FormModelsFacade()
        {
            if (!Directory.Exists(FormsPath))
            {
                Directory.CreateDirectory(FormsPath);
            }

            var batch = new CompositionBatch();
            var catalog = new DirectoryCatalog(HttpRuntime.BinDirectory);
            var container = new CompositionContainer(catalog);

            container.Compose(batch);

            ModelProviders = container.GetExportedValues<IFormModelsProvider>().ToList();
        }

        public static FormModel GetModel(string name)
        {
            return GetModels().SingleOrDefault(m => m.Name == name);
        }

        public static IEnumerable<FormModel> GetModels()
        {
            return ModelProviders.SelectMany(provider => provider.GetModels());
        }
    }
}