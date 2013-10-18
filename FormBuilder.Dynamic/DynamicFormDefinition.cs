using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public class DynamicFormDefinition
    {
        public FormModel Model { get; private set; }

        public string FormExecutor { get; set; }

        public string Name
        {
            get { return Model.Name; }
        }

        public DynamicFormDefinition(string name)
        {
            Model = new FormModel(name);
        }

        public DynamicFormDefinition(FormModel model)
        {
            Model = model;
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
                        case "InputElement": parseElementType(el, attrs); break;
                        case "ValidationRules": parseValidationRules(el, attrs); break;
                        case "DataSource": parseDataSource(el, attrs); break;
                        case "Dependencies": parseDependencies(el, attrs); break;
                    }
                }

                var formField = new FormField(model, fieldName, typeof(string), attrs);

                model.Fields.Add(formField);
            }

            model.OnSubmitHandler = () => { };

            var definition = new DynamicFormDefinition(model);

            var metaData = xml.Element("MetaData");
            if (metaData != null)
            {
                definition.FormExecutor = metaData.Element("FormExecutor").Attribute("functionName").Value;
            }

            return definition;
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
                var ctorParams = new Dictionary<string, object>();

                var typeString = rule.Attribute("type").Value;
                var type = Type.GetType(typeString);
                var ctor = type.GetConstructors().First();

                foreach (var param in ctor.GetParameters())
                {
                    var name = param.Name;
                    var valueString = rule.Attribute(name).Value;
                    object value = null;

                    if (param.ParameterType == typeof(int))
                    {
                        value = int.Parse(valueString);
                    }
                    else
                    {
                        value = valueString;
                    }

                    ctorParams.Add(name, value);
                }

                var ruleAttribute = (FormValidationAttribute)ctor.Invoke(ctorParams.Values.ToArray());

                attrs.Add(ruleAttribute);
            }
        }

        private static void parseElementType(XElement el, List<Attribute> attrs)
        {
            var typeString = el.Element("Type").Value;
            var type = Type.GetType(typeString);

            attrs.Add(new TypeBasedInputElementProviderAttribute(type));
        }
    }
}
