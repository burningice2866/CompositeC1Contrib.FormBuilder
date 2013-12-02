using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;
using CompositeC1Contrib.FormBuilder.Validation;
using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public class DynamicFormDefinition
    {
        public FormModel Model { get; private set; }
        public IDictionary<string, XElement> DefaultValues { get; private set; }
        public IList<FormSubmitHandler> SubmitHandlers { get; private set; }

        public string FormExecutor { get; set; }

        public string Name
        {
            get { return Model.Name; }
        }

        public DynamicFormDefinition(string name) : this(new FormModel(name)) { }

        public DynamicFormDefinition(FormModel model)
        {
            Model = model;
            DefaultValues = new Dictionary<string, XElement>();
            SubmitHandlers = new List<FormSubmitHandler>();
        }

        public static DynamicFormDefinition Parse(string name, XElement xml)
        {
            var model = new FormModel(name);

            var fields = xml.Element("Fields").Elements("Add");
            foreach (var f in fields)
            {
                var attrs = new List<Attribute>();
                var fieldName = f.Attribute("name").Value;

                var label = f.Attribute("label");
                if (label != null)
                {
                    attrs.Add(new FieldLabelAttribute(label.Value));
                }

                var help = f.Attribute("help");
                if (help != null)
                {
                    attrs.Add(new FieldHelpAttribute(help.Value));
                }

                foreach (var el in f.Elements())
                {
                    switch (el.Name.LocalName)
                    {
                        case "InputElement": parseInputElement(el, attrs); break;
                        case "ValidationRules": parseValidationRules(el, attrs); break;
                        case "DataSource": parseDataSource(el, attrs); break;
                        case "Dependencies": parseDependencies(el, attrs); break;
                    }
                }

                var formField = new FormField(model, fieldName, typeof(string), attrs);

                if (formField.InputElementType is CheckboxInputElementAttribute)
                {
                    formField.ValueType = typeof(bool);
                }
                else if (formField.InputElementType is CheckboxInputElementAttribute && formField.DataSource != null)
                {
                    formField.ValueType = typeof(IEnumerable<string>);
                }

                model.Fields.Add(formField);
            }

            var definition = new DynamicFormDefinition(model);

            parseMetaData(xml, definition);

            return definition;
        }

        private static void parseInputElement(XElement xml, List<Attribute> attrs)
        {
            var type = Type.GetType(xml.Attribute("type").Value);
            var inputElementAttr = (InputElementTypeAttribute)DynamicFormsFacade.DeserializeInstanceWithArgument(type, xml);

            attrs.Add(inputElementAttr);
        }

        private static void parseMetaData(XElement xml, DynamicFormDefinition definition)
        {
            var metaData = xml.Element("MetaData");
            if (metaData != null)
            {
                var formExecutorElement = metaData.Element("FormExecutor");
                if (formExecutorElement != null)
                {
                    definition.FormExecutor = formExecutorElement.Attribute("functionName").Value;
                }

                parseMetaDataDefaultValues(metaData, definition);
                parseMetaDataSubmitHandlers(metaData, definition);
            }
        }

        private static void parseMetaDataDefaultValues(XElement metaData, DynamicFormDefinition definition)
        {
            var defaultValuesElement = metaData.Element("DefaultValues");
            if (defaultValuesElement != null)
            {
                foreach (var handler in defaultValuesElement.Elements("Add"))
                {
                    var fieldName = handler.Attribute("field").Value;
                    var functionMarkup = XElement.Parse(handler.Attribute("functionMarkup").Value);

                    definition.DefaultValues.Add(fieldName, functionMarkup);
                }
            }
        }

        private static void parseMetaDataSubmitHandlers(XElement metaData, DynamicFormDefinition definition)
        {
            var submitHandlersElement = metaData.Element("SubmitHandlers");
            if (submitHandlersElement != null)
            {
                foreach (var handler in submitHandlersElement.Elements("Add"))
                {
                    var handlerName = handler.Attribute("Name").Value;
                    var typeString = handler.Attribute("Type").Value;
                    var type = Type.GetType(typeString);
                    var instance = (FormSubmitHandler)DynamicFormsFacade.DeserializeInstanceWithArgument(type, handler);

                    instance.Name = handlerName;

                    definition.SubmitHandlers.Add(instance);
                }
            }
        }

        private static void parseDependencies(XElement el, List<Attribute> attrs)
        {
            var dependencies = el.Elements("Add");
            foreach (var dep in dependencies)
            {
                var field = dep.Attribute("field").Value;
                var value = dep.Attribute("value").Value;
                var attribute = new DependsOnConstantAttribute(field, value);

                attrs.Add(attribute);
            }
        }

        private static void parseDataSource(XElement el, List<Attribute> attrs)
        {
            var typeString = el.Attribute("type").Value;
            var type = Type.GetType(typeString);

            if ((typeof(StringBasedDataSourceAttribute).IsAssignableFrom(type)))
            {
                var values = el.Element("values").Elements("item").Select(itm => itm.Attribute("value").Value).ToArray();
                var attribute = (Attribute)Activator.CreateInstance(type, new[] { values });

                attrs.Add(attribute);
            }
        }

        private static void parseValidationRules(XElement el, List<Attribute> attrs)
        {
            var rules = el.Elements("Add");
            foreach (var rule in rules)
            {
                var typeString = rule.Attribute("type").Value;
                var type = Type.GetType(typeString);

                var ruleAttribute = (FormValidationAttribute)DynamicFormsFacade.DeserializeInstanceWithArgument(type, rule);

                attrs.Add(ruleAttribute);
            }
        }
    }
}
