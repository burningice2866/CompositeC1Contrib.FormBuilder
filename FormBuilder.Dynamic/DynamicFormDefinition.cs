using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Composite.Core.Xml;

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
        public XhtmlDocument IntroText { get; set; }
        public XhtmlDocument SuccessResponse { get; set; }

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
                        case "InputElement": ParseInputElement(el, attrs); break;
                        case "ValidationRules": ParseValidationRules(el, attrs); break;
                        case "DataSource": ParseDataSource(el, attrs); break;
                        case "Dependencies": ParseDependencies(el, attrs); break;
                    }
                }

                var formField = new FormField(model, fieldName, typeof(string), attrs);

                var isReadOnly = f.Attribute("isReadOnly");
                if (isReadOnly != null)
                {
                    formField.IsReadOnly = bool.Parse(isReadOnly.Value);
                }

                SetFieldValueType(formField);

                model.Fields.Add(formField);
            }

            var definition = new DynamicFormDefinition(model);

            ParseMetaData(xml, definition);

            return definition;
        }

        private static void SetFieldValueType(FormField formField)
        {
            if (formField.InputElementType is CheckboxInputElementAttribute)
            {
                formField.ValueType = typeof(bool);
            }

            if (formField.InputElementType is CheckboxInputElementAttribute && formField.DataSource != null)
            {
                formField.ValueType = typeof(IEnumerable<string>);
            }

            if (formField.InputElementType is FileuploadInputElementAttribute)
            {
                formField.ValueType = typeof(FormFile);
            }
        }

        private static void ParseInputElement(XElement xml, IList<Attribute> attrs)
        {
            var type = Type.GetType(xml.Attribute("type").Value);
            var inputElementAttr = (InputElementTypeAttribute)DynamicFormsFacade.DeserializeInstanceWithArgument(type, xml);

            attrs.Add(inputElementAttr);
        }

        private static void ParseMetaData(XElement xml, DynamicFormDefinition definition)
        {
            var metaData = xml.Element("MetaData");
            if (metaData == null)
            {
                return;
            }

            var formExecutorElement = metaData.Element("FormExecutor");
            if (formExecutorElement != null)
            {
                definition.FormExecutor = formExecutorElement.Attribute("functionName").Value;
            }

            var forceHttpsConnectionElement = metaData.Element("forceHttpsConnection");
            if (forceHttpsConnectionElement != null)
            {
                definition.Model.Attributes.Add(new ForceHttpsConnectionAttribute());
            }

            var requiresCaptchaElement = metaData.Element("requiresCaptcha");
            if (requiresCaptchaElement != null)
            {
                definition.Model.Attributes.Add(new RequiresCaptchaAttribute());
            }

            var layoutElement = metaData.Element("Layout");
            if (layoutElement != null)
            {
                var label = layoutElement.Attribute("submitButtonLabel").Value;
                var introText = layoutElement.Element("introText");
                var successResponse = layoutElement.Element("successResponse");

                definition.Model.Attributes.Add(new SubmitButtonLabelAttribute(label));

                if (introText != null)
                {
                    definition.IntroText = XhtmlDocument.Parse(introText.Value);
                }
                else
                {
                    definition.IntroText = new XhtmlDocument();
                }

                if (successResponse != null)
                {
                    definition.SuccessResponse = XhtmlDocument.Parse(successResponse.Value);
                }
                else
                {
                    definition.SuccessResponse = new XhtmlDocument();
                }
            }

            ParseMetaDataDefaultValues(metaData, definition);
            ParseMetaDataSubmitHandlers(metaData, definition);
        }

        private static void ParseMetaDataDefaultValues(XElement metaData, DynamicFormDefinition definition)
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

        private static void ParseMetaDataSubmitHandlers(XElement metaData, DynamicFormDefinition definition)
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

        private static void ParseDependencies(XElement el, List<Attribute> attrs)
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

        private static void ParseDataSource(XElement el, List<Attribute> attrs)
        {
            var typeString = el.Attribute("type").Value;
            var type = Type.GetType(typeString);

            if (!typeof(StringBasedDataSourceAttribute).IsAssignableFrom(type))
            {
                return;
            }

            var values = el.Element("values").Elements("item").Select(itm => itm.Attribute("value").Value).ToArray();
            var attribute = (Attribute)Activator.CreateInstance(type, new object[] { values });

            attrs.Add(attribute);
        }

        private static void ParseValidationRules(XElement el, List<Attribute> attrs)
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
