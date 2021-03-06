﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public abstract class XmlDefinitionSerializer
    {
        public static XmlDefinitionSerializer GetSerializer(string name)
        {
            var file = Path.Combine(ModelsFacade.RootPath, name, "DynamicDefinition.xml");
            if (!File.Exists(file))
            {
                throw new InvalidOperationException("Can't get serializer of a form that hasn't previously been saved");
            }

            var xml = XElement.Load(file);

            return GetSerializer(xml);
        }

        public static XmlDefinitionSerializer GetSerializer(XElement xml)
        {
            switch (xml.Name.LocalName)
            {
                case "FormBuilder.DynamicForm": return new FormXmlSerializer();
                case "FormBuilder.Wizard": return new WizardXmlSerializer();
            }

            throw new NotSupportedException("Unrecognized form type, cannot seralize it");
        }

        public virtual void Save(IDynamicDefinition form)
        {
            ModelsFacade.NotifyFormChanges();
        }

        public abstract IDynamicDefinition Load(string name, XElement xml);

        protected static void SaveDefinitionFile(string name, XElement xml)
        {
            var dir = Path.Combine(ModelsFacade.RootPath, name);
            var file = Path.Combine(dir, "DynamicDefinition.xml");

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            xml.Save(file);
        }

        protected void SaveMetaData(IDynamicDefinition definition, XElement root)
        {
            if (definition.SubmitHandlers.Any())
            {
                var submitHandlers = new XElement("SubmitHandlers");

                foreach (var handler in definition.SubmitHandlers)
                {
                    var qualifiedName = handler.GetType().AssemblyQualifiedName;
                    if (qualifiedName == null)
                    {
                        continue;
                    }

                    var handlerElement = new XElement("Add",
                        new XAttribute("Name", handler.Name),
                        new XAttribute("Type", qualifiedName));

                    handler.Save(definition, handlerElement);

                    submitHandlers.Add(handlerElement);
                }

                root.Add(submitHandlers);
            }

            if (definition.Settings != null)
            {
                var qualifiedName = definition.Settings.GetType().AssemblyQualifiedName;
                if (qualifiedName != null)
                {
                    var settingsElement = new XElement("FormSettings",
                        new XAttribute("Type", qualifiedName));

                    SerializeInstanceWithArgument(definition.Settings, settingsElement);

                    root.Add(settingsElement);
                }
            }
        }

        protected static void ParseSubmitHandlers(XElement root, IDynamicDefinition definition)
        {
            var submitHandlersElement = root.Element("SubmitHandlers");
            if (submitHandlersElement == null)
            {
                return;
            }

            foreach (var handler in submitHandlersElement.Elements("Add"))
            {
                var typeString = handler.Attribute("Type").Value;

                var type = Type.GetType(typeString);
                if (type == null)
                {
                    if (typeString.StartsWith("CompositeC1Contrib.FormBuilder.Dynamic.Wizard.SubmitHandlers.EmailSubmitHandler"))
                    {
                        type = typeof(EmailSubmitHandler);
                    }
                }

                if (type == null)
                {
                    continue;
                }

                var instance = (FormSubmitHandler)XElementHelper.DeserializeInstanceWithArgument(type, handler);

                instance.Load(definition, handler);
                definition.SubmitHandlers.Add(instance);
            }
        }

        protected static void ParseFormSettings(XElement metaData, IDynamicDefinition definition)
        {
            var formSettingsElement = metaData.Element("FormSettings");
            if (formSettingsElement == null)
            {
                return;
            }

            var typeString = formSettingsElement.Attribute("Type").Value;

            var type = Type.GetType(typeString);
            if (type == null)
            {
                return;
            }

            var instance = (IFormSettings)XElementHelper.DeserializeInstanceWithArgument(type, formSettingsElement);

            definition.Settings = instance;
        }

        public void SerializeInstanceWithArgument(object instance, XElement element)
        {
            var type = instance.GetType();

            foreach (var prop in type.GetProperties().Where(p => p.CanRead && p.CanWrite))
            {
                var value = prop.GetValue(instance, null);
                if (value != null)
                {
                    element.Add(new XAttribute(prop.Name.ToLowerInvariant(), value));
                }
            }
        }
    }
}
