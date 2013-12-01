using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Composite;

using CompositeC1Contrib.FormBuilder.Configuration;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console;

namespace CompositeC1Contrib.FormBuilder.Dynamic.Configuration
{
    public class DynamicFormBuilderConfiguration : IPluginConfiguration
    {
        public IList<InputElementHandler> InputElementHandlers { get; private set; }

        public DynamicFormBuilderConfiguration()
        {
            InputElementHandlers = new List<InputElementHandler>();
        }

        public void Load(XmlNode element)
        {
            var inputElementHandlers = element.FirstChild;
            foreach (XmlNode add in inputElementHandlers.ChildNodes)
            {
                var attributes = add.Attributes.Cast<XmlAttribute>().ToDictionary(attr => attr.Name);

                var name = attributes["name"].Value;
                var type = Type.GetType(attributes["type"].Value);

                Verify.IsNotNull(type, "Unrecognized inputelement type");

                var inputElementHandler = new InputElementHandler
                {
                    Name = name,
                    Type = type
                };

                if (attributes.ContainsKey("settingsHandler"))
                {
                    var settingsHandler = Type.GetType(attributes["settingsHandler"].Value);

                    Verify.IsNotNull(settingsHandler, "Unrecognized settingshandler");

                    inputElementHandler.SettingsHandler = settingsHandler;
                }

                InputElementHandlers.Add(inputElementHandler);
            }
        }
    }
}
