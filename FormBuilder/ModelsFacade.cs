﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;

using Composite.Core.IO;

using CompositeC1Contrib.Composition;

namespace CompositeC1Contrib.FormBuilder
{
    public static class ModelsFacade
    {
        private static readonly object Lock = new object();
        private static IDictionary<string, ProviderModelContainer> _cachedList;

        public static readonly string RootPath = HostingEnvironment.MapPath("~/App_Data/FormBuilder");

        public static event EventHandler FormChanges;

        public static IEnumerable<IModelsProvider> Providers { get; private set; }

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

            return GetCachedList().TryGetValue(name, out container) ? container.Model : null;
        }

        public static IEnumerable<ProviderModelContainer> GetModels()
        {
            return GetCachedList().Values;
        }

        private static IDictionary<string, ProviderModelContainer> GetCachedList()
        {
            if (_cachedList == null)
            {
                lock (Lock)
                {
                    if (_cachedList == null)
                    {
                        _cachedList = Providers.SelectMany(provider => provider.GetModels()).ToDictionary(o => o.Model.Name);
                    }
                }
            }

            return _cachedList;
        }

        public static void NotifyFormChanges()
        {
            _cachedList = null;

            if (FormChanges != null)
            {
                FormChanges(null, EventArgs.Empty);
            }
        }
    }
}