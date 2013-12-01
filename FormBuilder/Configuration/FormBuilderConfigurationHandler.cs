using System;
using System.Configuration;
using System.Linq;
using System.Xml;

namespace CompositeC1Contrib.FormBuilder.Configuration
{
    public class FormBuilderConfigurationHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new FormBuilderConfiguration();

            var defaultFunctionExecutor = section.Attributes.Cast<XmlAttribute>().SingleOrDefault(attr => attr.Name == "defaultFunctionExecutor");
            if (defaultFunctionExecutor != null)
            {
                config.DefaultFunctionExecutor = defaultFunctionExecutor.Value;
            }
            else
            {

                config.DefaultFunctionExecutor = "FormBuilder.StandardFormExecutor";
            }

            foreach (XmlNode element in section.ChildNodes)
            {
                var sType = element.Attributes.Cast<XmlAttribute>().SingleOrDefault(attr => attr.Name == "type");
                if (sType != null)
                {
                    var type = Type.GetType(sType.Value);
                    if (type != null)
                    {
                        var plugin = (IPluginConfiguration)Activator.CreateInstance(type);

                        plugin.Load(element);

                        config.Plugins.Add(element.Name, plugin);
                    }
                }
            }

            return config;
        }
    }
}
