using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml.Linq;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public class DynamicFormsFacade
    {
        private static IList<Action> _formChangeNotifications = new List<Action>();
        private static string _basePath = HostingEnvironment.MapPath("~/App_Data/FormBuilder/FormDefinitions");

        public static void SubscribeToFormChanges(Action notify)
        {
            _formChangeNotifications.Add(notify);
        }        

        public static FormModel GetFormByName(string name)
        {
            var file = Path.Combine(_basePath, name + ".xml");

            return FromBaseForm(file, name);
        }

        public static IEnumerable<FormModel> GetFormDefinitions()
        {
            var files = Directory.GetFiles(_basePath);

            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);

                yield return FromBaseForm(file, name);
            }
        }

        public static void SaveForm(FormModel model)
        {
            var file = Path.Combine(_basePath, model.Name + ".xml");
            var newForm = !File.Exists(file);

            var root = new XElement("FormBuilder.DynamicForms");
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

                if (field.InputTypeHandler != null)
                {
                    add.Add(new XElement("InputElement",
                        new XElement("Type", field.InputTypeHandler.GetType().AssemblyQualifiedName)));
                }

                if (field.ValidationAttributes.Any())
                {
                    var validationRules = new XElement("ValidationRules");

                    foreach (var rule in field.ValidationAttributes)
                    {
                        var ruleType = rule.GetType();
                        var ruleElement = new XElement("Add", new XAttribute("type", ruleType.AssemblyQualifiedName));

                        foreach (var prop in ruleType.GetProperties())
                        {
                            var value = prop.GetValue(rule, null).ToString();

                            ruleElement.Add(new XAttribute(prop.Name.ToLowerInvariant(), value));
                        }

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

            if (newForm)
            {
                NotifyFormChanges();
            }
        }

        public static void DeleteModel(FormModel model)
        {
            var file = Path.Combine(_basePath, model.Name + ".xml");

            File.Delete(file);
            NotifyFormChanges();
        }

        private static FormModel FromBaseForm(string file, string name)
        {
            if (!File.Exists(file))
            {
                return null;
            }

            var xml = XElement.Load(file);
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

            return model;
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

        private static void NotifyFormChanges()
        {
            foreach (var action in _formChangeNotifications)
            {
                action();
            }
        }
    }
}
