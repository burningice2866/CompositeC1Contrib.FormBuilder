﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml.Linq;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public class DynamicFormsFacade
    {
        private static IList<Action> _formChangeNotifications = new List<Action>();
        private static string _basePath = HostingEnvironment.MapPath("~/App_Data/FormBuilder/FormDefinitions");

        static DynamicFormsFacade()
        {
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public static void SubscribeToFormChanges(Action notify)
        {
            _formChangeNotifications.Add(notify);
        }

        public static DynamicFormDefinition GetFormByName(string name)
        {
            var file = Path.Combine(_basePath, name + ".xml");

            return FromBaseForm(file, name);
        }

        public static DynamicFormDefinition CopyFormByName(string name, string newName)
        {
            var file = Path.Combine(_basePath, name + ".xml");

            return FromBaseForm(file, newName);
        }

        public static DynamicFormDefinition FromBaseForm(string file, string name)
        {
            if (!File.Exists(file))
            {
                return null;
            }

            var xml = XElement.Load(file);

            return DynamicFormDefinition.Parse(name, xml);
        }

        public static IEnumerable<DynamicFormDefinition> GetFormDefinitions()
        {
            var files = Directory.GetFiles(_basePath);

            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);

                yield return FromBaseForm(file, name);
            }
        }

        public static void SaveForm(DynamicFormDefinition definition)
        {
            var model = definition.Model;
            var file = Path.Combine(_basePath, definition.Name + ".xml");

            var root = new XElement("FormBuilder.DynamicForm",
                new XAttribute("name", definition.Name));

            var metaData = new XElement("MetaData");

            if (!String.IsNullOrEmpty(definition.FormExecutor))
            {
                metaData.Add(new XElement("FormExecutor",
                    new XAttribute("functionName", definition.FormExecutor)));
            }

            if (definition.DefaultValues.Any()) 
            {
                var defaultValues = new XElement("DefaultValues");

                foreach (var kvp in definition.DefaultValues)
                {
                    var defaultValueElement = new XElement("Add",
                        new XAttribute("field", kvp.Key),
                        new XAttribute("functionMarkup", kvp.Value.ToString()));

                    defaultValues.Add(defaultValueElement);
                }

                metaData.Add(defaultValues);
            }

            if (definition.SubmitHandlers.Any())
            {
                var submitHandlers = new XElement("SubmitHandlers");

                foreach (var handler in definition.SubmitHandlers)
                {
                    var handlerElement = new XElement("Add",
                        new XAttribute("Name", handler.Name),
                        new XAttribute("Type", handler.GetType().AssemblyQualifiedName));

                    if (handler is EmailSubmitHandler)
                    {
                        var emailHandler = (EmailSubmitHandler)handler;

                        handlerElement.Add(new XAttribute("IncludeAttachments", emailHandler.IncludeAttachments));
                        handlerElement.Add(new XAttribute("From", emailHandler.From ?? String.Empty));
                        handlerElement.Add(new XAttribute("To", emailHandler.To ?? String.Empty));
                        handlerElement.Add(new XAttribute("Cc", emailHandler.Cc ?? String.Empty));
                        handlerElement.Add(new XAttribute("Bcc", emailHandler.Bcc ?? String.Empty));

                        handlerElement.Add(new XElement("Subject", emailHandler.Subject ?? String.Empty));
                        handlerElement.Add(new XElement("Body", emailHandler.Body ?? String.Empty));
                    }

                    submitHandlers.Add(handlerElement);
                }

                metaData.Add(submitHandlers);
            }

            if (metaData.HasElements)
            {
                root.Add(metaData);
            }

            var fields = new XElement("Fields");

            foreach (var field in model.Fields)
            {
                var add = new XElement("Add", new XAttribute("name", field.Name));

                if (field.Label != null)
                {
                    add.Add(new XAttribute("label", field.Label.Label));
                }

                if (!String.IsNullOrEmpty(field.Help))
                {
                    add.Add(new XAttribute("help", field.Help));
                }

                if (field.InputElementType != null)
                {
                    var inputElement = new XElement("InputElement");
                    var inputElementType = field.InputElementType.GetType();

                    inputElement.Add(new XAttribute("type", inputElementType.AssemblyQualifiedName));

                    SerializeInstanceWithArgument(field.InputElementType, inputElement);

                    add.Add(inputElement);
                }

                if (field.ValidationAttributes.Any())
                {
                    var validationRules = new XElement("ValidationRules");

                    foreach (var rule in field.ValidationAttributes)
                    {
                        var ruleType = rule.GetType();
                        var ruleElement = new XElement("Add", new XAttribute("type", ruleType.AssemblyQualifiedName));

                        SerializeInstanceWithArgument(rule, ruleElement);

                        validationRules.Add(ruleElement);
                    }

                    add.Add(validationRules);
                }

                if (field.DependencyAttributes.Any())
                {
                    var dependencies = new XElement("Dependencies");

                    foreach (var dep in field.DependencyAttributes)
                    {
                        var values = String.Join(",", dep.RequiredFieldValues());

                        dependencies.Add(new XElement("Add",
                            new XAttribute("field", dep.ReadFromFieldName),
                            new XAttribute("value", values)));
                    }

                    add.Add(dependencies);
                }

                var dataSourceAttribute = field.Attributes.OfType<DataSourceAttribute>().FirstOrDefault();
                if (dataSourceAttribute != null)
                {
                    var datasource = new XElement("DataSource", new XAttribute("type", dataSourceAttribute.GetType().AssemblyQualifiedName));

                    var stringBasedDataSourceAttribute = dataSourceAttribute as StringBasedDataSourceAttribute;
                    if (stringBasedDataSourceAttribute != null)
                    {
                        var values = new XElement("values");

                        foreach (var s in stringBasedDataSourceAttribute.Values)
                        {
                            values.Add(new XElement("item", new XAttribute("value", s)));
                        }

                        datasource.Add(values);
                    }

                    add.Add(datasource);
                }

                fields.Add(add);
            }

            root.Add(fields);
            root.Save(file);

            NotifyFormChanges();
        }

        public static void SerializeInstanceWithArgument(object instance, XElement element)
        {
            var type = instance.GetType();

            foreach (var prop in type.GetProperties())
            {
                var value = prop.GetValue(instance, null).ToString();

                element.Add(new XAttribute(prop.Name.ToLowerInvariant(), value));
            }
        }

        public static object DeserializeInstanceWithArgument(Type type, XElement element)
        {
            var ctors = type.GetConstructors().ToDictionary(ctor => ctor, ctor => ctor.GetParameters()).OrderByDescending(o => o.Value.Count());

            foreach (var kvp in ctors)
            {
                var parameters = kvp.Value;
                var ctorParams = new Dictionary<string, object>();

                foreach (var param in parameters)
                {
                    var attr = element.Attributes().SingleOrDefault(o => o.Name.LocalName.Equals(param.Name, StringComparison.OrdinalIgnoreCase));
                    if (attr == null)
                    {
                        break;
                    }

                    var name = param.Name;
                    object value = Convert.ChangeType(attr.Value, param.ParameterType);

                    ctorParams.Add(name, value);
                }

                if (ctorParams.Count == parameters.Length)
                {
                    return kvp.Key.Invoke(ctorParams.Values.ToArray());
                }
            }

            var instance = Activator.CreateInstance(type);

            foreach (var prop in type.GetProperties())
            {
                var attr = element.Attributes().SingleOrDefault(o => o.Name.LocalName.Equals(prop.Name, StringComparison.OrdinalIgnoreCase));
                if (attr != null)
                {
                    prop.SetValue(instance, Convert.ChangeType(attr.Value, prop.PropertyType), null);

                    continue;
                }

                var node = element.Elements().SingleOrDefault(o => o.Name.LocalName.Equals(prop.Name, StringComparison.OrdinalIgnoreCase));
                if (node != null)
                {
                    prop.SetValue(instance, Convert.ChangeType(element.Value, prop.PropertyType), null);

                    continue;
                }
            }

            return instance;
        }

        public static void DeleteModel(DynamicFormDefinition definition)
        {
            var file = Path.Combine(_basePath, definition.Name + ".xml");

            File.Delete(file);
            NotifyFormChanges();
        }

        private static void NotifyFormChanges()
        {
            foreach (var action in _formChangeNotifications)
            {
                action();
            }
        }
    }
}
