using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace CompositeC1Contrib.FormBuilder
{
    public static class FormModelsFacade
    {
        private static readonly IList<IFormModelsProvider> ModelProviders = new List<IFormModelsProvider>();
        public static readonly string FormsPath = HostingEnvironment.MapPath("~/App_Data/FormBuilder");

        static FormModelsFacade()
        {
            if (!Directory.Exists(FormsPath))
            {
                Directory.CreateDirectory(FormsPath);
            }

            var asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in asms)
            {
                try
                {
                    var types = asm.GetTypes()
                        .Where(t => typeof(IFormModelsProvider).IsAssignableFrom(t) && !t.IsInterface);

                    foreach (var instance in types.Select(t => (IFormModelsProvider)Activator.CreateInstance(t)))
                    {
                        ModelProviders.Add(instance);
                    }
                }
                catch { }
            }
        }

        public static IEnumerable<FormModel> GetModels()
        {
            return ModelProviders.SelectMany(provider => provider.GetModels());
        }
    }
}