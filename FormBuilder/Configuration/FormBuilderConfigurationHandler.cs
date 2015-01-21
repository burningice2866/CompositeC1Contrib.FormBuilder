using System;
using System.Configuration;
using System.Linq;
using System.Xml;

using CompositeC1Contrib.FormBuilder.Web.UI.FormRenderers;

namespace CompositeC1Contrib.FormBuilder.Configuration
{
    public class FormBuilderConfigurationHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            var sectionAttibutes = section.Attributes.Cast<XmlAttribute>().ToList();

            var defaultFunctionExecutor = sectionAttibutes.SingleOrDefault(attr => attr.Name == "defaultFunctionExecutor");
            var rendererImplementation = sectionAttibutes.SingleOrDefault(attr => attr.Name == "rendererImplementation");

            var config = new FormBuilderConfiguration
            {
                DefaultFunctionExecutor = defaultFunctionExecutor != null ? defaultFunctionExecutor.Value : "FormBuilder.StandardFormExecutor",
                RendererImplementation = rendererImplementation != null ? Type.GetType(rendererImplementation.Value) : typeof(Bootstrap2FormRenderer)
            };

            foreach (XmlNode element in section.ChildNodes)
            {
                if (element.Name == "captcha")
                {
                    config.Captcha = CaptchaConfiguration.FromXml(element);

                    continue;
                }

                var elementAttributes = element.Attributes.Cast<XmlAttribute>();
                var sType = elementAttributes.Single(attr => attr.Name == "type").Value;

                var type = Type.GetType(sType);
                if (type == null)
                {
                    continue;
                }

                var plugin = (IPluginConfiguration)Activator.CreateInstance(type);

                plugin.Initialize(element);

                config.Plugins.Add(element.Name, plugin);
            }

            return config;
        }
    }
}
