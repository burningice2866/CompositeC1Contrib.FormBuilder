using System;
using System.Globalization;
using System.Linq;
using System.Resources;

namespace CompositeC1Contrib.FormBuilder
{
    public static class ResourceFacade
    {
        public static ResourceManager DefaultResourceManager = new ResourceManager("CompositeC1Contrib.FormBuilder.Strings", typeof(ResourceFacade).Assembly);

        private static readonly ResourceManager ResourceManager;

        static ResourceFacade()
        {
            var resourceManagerType = Type.GetType("CompositeC1Contrib.Localization.C1ResourceManager, CompositeC1Contrib.Localization");
            if (resourceManagerType == null)
            {
                return;
            }

            var resourceManagerConstructor = (from constructor in resourceManagerType.GetConstructors()
                                              let parameters = constructor.GetParameters()
                                              where parameters.Length == 1 && parameters[0].ParameterType == typeof(string)
                                              select constructor).FirstOrDefault();

            if (resourceManagerConstructor != null)
            {
                ResourceManager = Activator.CreateInstance(resourceManagerType, "FormBuilder") as ResourceManager;
            }
        }

        public static ResourceManager GetResourceManager()
        {
            return ResourceManager;
        }

        public static IResourceWriter GetResourceWriter(CultureInfo culture)
        {
            var resourceManager = GetResourceManager();
            if (resourceManager == null)
            {
                return null;
            }

            var resourceSet = resourceManager.GetResourceSet(culture, true, false);
            var resourceWriterType = resourceSet.GetDefaultWriter();

            if (resourceWriterType == typeof(ResourceWriter))
            {
                return null;
            }

            var validConstructor = (from constructor in resourceWriterType.GetConstructors()
                                    let parameters = constructor.GetParameters()
                                    where parameters.Length == 2
                                          && parameters.Any(p => p.ParameterType == typeof(string))
                                          && parameters.Any(p => p.ParameterType == typeof(CultureInfo))
                                    select new
                                    {
                                        Constructor = constructor,
                                        Parameters = parameters
                                    }).FirstOrDefault();

            if (validConstructor == null)
            {
                return null;
            }

            var args = new object[]
            {
                "FormBuilder",
                culture
            }.ToDictionary(_ => _.GetType());

            var orderedArguments = validConstructor.Parameters.Select(p => args[p.ParameterType]).ToArray();

            return validConstructor.Constructor.Invoke(orderedArguments) as IResourceWriter;
        }
    }
}
