using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Composite;

using CompositeC1Contrib.FormBuilder.Configuration;

namespace CompositeC1Contrib.FormBuilder.Dynamic.Wizard.Configuration
{
    public class WizardFormBuilderConfiguration : IPluginConfiguration
    {
        public IList<SubmitHandlerElement> SubmitHandlers { get; private set; }

        public WizardFormBuilderConfiguration()
        {
            SubmitHandlers = new List<SubmitHandlerElement>();
        }

        public void Load(XmlNode element)
        {
            var submitHandlers = element.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.LocalName == "submitHandlers");
            if (submitHandlers != null)
            {
                foreach (XmlNode add in submitHandlers.ChildNodes)
                {
                    var attributes = add.Attributes.Cast<XmlAttribute>().ToDictionary(attr => attr.Name);

                    var name = attributes["name"].Value;
                    var type = Type.GetType(attributes["type"].Value);
                    var allowMultiple = attributes.ContainsKey("allowMultiple") && bool.Parse(attributes["allowMultiple"].Value);

                    Verify.IsNotNull(type, "Unrecognized submithandler type");

                    SubmitHandlers.Add(new SubmitHandlerElement
                    {
                        Name = name,
                        Type = type,
                        AllowMultiple = allowMultiple
                    });
                }
            }
        }
    }
}
