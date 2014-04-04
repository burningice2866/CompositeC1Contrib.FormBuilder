using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace CompositeC1Contrib.FormBuilder
{
    public static class FormModelsFacade
    {
        public static readonly string FormsPath = HostingEnvironment.MapPath("~/App_Data/FormBuilder");

        private static IList<IFormModelsProvider> _modelProviders;

        static FormModelsFacade()
        {
            if (!Directory.Exists(FormsPath))
            {
                Directory.CreateDirectory(FormsPath);
            }
        }

        public static FormModel GetModel(string name)
        {
            return GetModels().SingleOrDefault(m => m.Name == name);
        }

        public static IEnumerable<FormModel> GetModels()
        {
            if (_modelProviders == null)
            {
                _modelProviders = LoadFormModelProviders();
            }

            return _modelProviders.SelectMany(provider => provider.GetModels());
        }

        private static IList<IFormModelsProvider> LoadFormModelProviders()
        {
            var modelProviders = new List<IFormModelsProvider>();

            var asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in asms)
            {
                try
                {
                    var types = asm.GetTypes().Where(t => typeof(IFormModelsProvider).IsAssignableFrom(t) && !t.IsInterface);

                    modelProviders.AddRange(types.Select(t => (IFormModelsProvider)Activator.CreateInstance(t)));
                }
                catch { }
            }

            return modelProviders;
        }
    }
}