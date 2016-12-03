using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;

using Composite.Core.IO;

using CompositeC1Contrib.Composition;

namespace CompositeC1Contrib.FormBuilder
{
    public static class ModelsFacade
    {
        private static Lazy<IDictionary<string, ProviderModelContainer>> _cachedList = new Lazy<IDictionary<string, ProviderModelContainer>>(() =>
        {
            return Providers.SelectMany(provider => provider.GetModels()).ToDictionary(o => o.Model.Name);
        });

        public static readonly string RootPath = HostingEnvironment.MapPath("~/App_Data/FormBuilder");

        public static event EventHandler FormChanges;

        public static IEnumerable<IModelsProvider> Providers { get; }

        static ModelsFacade()
        {
            Providers = CompositionContainerFacade.GetExportedValues<IModelsProvider>().ToList();

            if (!C1Directory.Exists(RootPath))
            {
                C1Directory.CreateDirectory(RootPath);
            }
        }

        public static T GetModel<T>(string name) where T : class, IModel
        {
            var model = GetModel(name) as T;
            if (model == null)
            {
                throw new InvalidOperationException(name + " is of the wrong model type");
            }

            return model;
        }

        public static IModel GetModel(string name)
        {
            ProviderModelContainer container;

            return _cachedList.Value.TryGetValue(name, out container) ? container.Model : null;
        }

        public static IEnumerable<ProviderModelContainer> GetModels()
        {
            return _cachedList.Value.Values;
        }

        public static void NotifyFormChanges()
        {
            _cachedList = new Lazy<IDictionary<string, ProviderModelContainer>>(() =>
            {
                return Providers.SelectMany(provider => provider.GetModels()).ToDictionary(o => o.Model.Name);
            });

            FormChanges?.Invoke(null, EventArgs.Empty);
        }
    }
}