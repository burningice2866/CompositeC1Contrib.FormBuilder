using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Configuration;
using System.Xml;

using CompositeC1Contrib.FormBuilder.Web;
using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder.Configuration
{
    public class CaptchaConfiguration
    {
        private static readonly ProviderSettings DefaultSettings = new ProviderSettings("C1", typeof(CompositeC1CaptchaProvider).AssemblyQualifiedName);

        public string DefaultProvider { get; set; }
        public ProviderCollection Providers { get; private set; }

        public CaptchaConfiguration()
        {
            DefaultProvider = DefaultSettings.Name;

            Providers = new ProviderCollection
            {
                InstantiateProvider(DefaultSettings)
            };
        }

        public static CaptchaConfiguration FromXml(XmlNode xml)
        {
            var config = new CaptchaConfiguration();

            var defaultProviderAttribute = xml.Attributes.Cast<XmlAttribute>().SingleOrDefault(a => a.Name == "defaultProvider");
            if (defaultProviderAttribute != null)
            {
                config.DefaultProvider = defaultProviderAttribute.Value;
            }

            foreach (XmlNode element in xml.ChildNodes)
            {
                if (element.Name == "providers")
                {
                    HandleProvidersElement(element, config);
                }
            }

            return config;
        }

        private static void HandleProvidersElement(XmlNode providers, CaptchaConfiguration config)
        {
            config.Providers = new ProviderCollection();

            foreach (XmlNode element in providers.ChildNodes)
            {
                var attributes = element.Attributes.Cast<XmlAttribute>().ToDictionary(attr => attr.Name);

                switch (element.Name)
                {
                    case "add":
                        {
                            var name = attributes["name"].Value;
                            var type = attributes["type"].Value;

                            var settings = new ProviderSettings(name, type);

                            foreach (var attr in attributes.Where(attr => attr.Key != "name" && attr.Key != type))
                            {
                                settings.Parameters.Add(attr.Key, attr.Value.Value);
                            }

                            var provider = InstantiateProvider(settings);

                            config.Providers.Add(provider);
                        }

                        break;

                    case "remove":
                        {
                            var name = attributes["name"].Value;

                            config.Providers.Remove(name);
                        }

                        break;

                    case "clear":
                        {
                            config.Providers.Clear();
                        }

                        break;
                }
            }
        }

        private static ProviderBase InstantiateProvider(ProviderSettings settings)
        {
            return ProvidersHelper.InstantiateProvider(settings, typeof(CaptchaProvider));
        }
    }
}
