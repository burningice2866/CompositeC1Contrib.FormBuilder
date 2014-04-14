using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public class DynamicFormsFacade
    {
        private static readonly IList<Action> FormChangeNotifications = new List<Action>();

        public static void SubscribeToFormChanges(Action notify)
        {
            FormChangeNotifications.Add(notify);
        }

        public static DynamicFormDefinition GetFormByName(string name)
        {
            var file = Path.Combine(FormModelsFacade.FormsPath, name, "DynamicDefinition.xml");

            return FromBaseForm(file, name);
        }

        public static DynamicFormDefinition CopyFormByName(string name, string newName)
        {
            var file = Path.Combine(FormModelsFacade.FormsPath, name, "DynamicDefinition.xml");

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
            var files = Directory.GetFiles(FormModelsFacade.FormsPath, "DynamicDefinition.xml", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var folder = Path.GetDirectoryName(file);
                var name = new DirectoryInfo(folder).Name;

                yield return FromBaseForm(file, name);
            }
        }

        public static void SaveForm(DynamicFormDefinition definition)
        {
            var model = definition.Model;
            var dir = Path.Combine(FormModelsFacade.FormsPath, definition.Name);
            var file = Path.Combine(dir, "DynamicDefinition.xml");

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var root = new XElement("FormBuilder.DynamicForm",
                new XAttribute("name", definition.Name));

            var metaData = new XElement("MetaData");

            if (!String.IsNullOrEmpty(definition.FormExecutor))
            {
                metaData.Add(new XElement("FormExecutor",
                    new XAttribute("functionName", definition.FormExecutor)));
            }

            if (model.Attributes.OfType<ForceHttpsConnectionAttribute>().Any())
            {
                metaData.Add(new XElement("forceHttpsConnection"));
            }

            if (model.Attributes.OfType<RequiresCaptchaAttribute>().Any())
            {
                metaData.Add(new XElement("requiresCaptcha"));
            }

            var layout = new XElement("Layout",
                    new XAttribute("submitButtonLabel", definition.Model.SubmitButtonLabel));

            if (definition.IntroText != null)
            {
                layout.Add(new XElement("introText", definition.IntroText.ToString()));
            }

            if (definition.IntroText != null)
            {
                layout.Add(new XElement("successResponse", definition.SuccessResponse.ToString()));
            }

            metaData.Add(layout);

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

                    var emailSubmitHandler = handler as EmailSubmitHandler;
                    if (emailSubmitHandler != null)
                    {
                        handlerElement.Add(new XAttribute("IncludeAttachments", emailSubmitHandler.IncludeAttachments));
                        handlerElement.Add(new XAttribute("From", emailSubmitHandler.From ?? String.Empty));
                        handlerElement.Add(new XAttribute("To", emailSubmitHandler.To ?? String.Empty));
                        handlerElement.Add(new XAttribute("Cc", emailSubmitHandler.Cc ?? String.Empty));
                        handlerElement.Add(new XAttribute("Bcc", emailSubmitHandler.Bcc ?? String.Empty));

                        handlerElement.Add(new XElement("Subject", emailSubmitHandler.Subject ?? String.Empty));
                        handlerElement.Add(new XElement("Body", emailSubmitHandler.Body ?? String.Empty));
                    }

                    var saveFormSubmitHandler = handler as SaveFormSubmitHandler;
                    if (saveFormSubmitHandler != null)
                    {
                        handlerElement.Add(new XAttribute("IncludeAttachments", saveFormSubmitHandler.IncludeAttachments));
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
                var add = new XElement("Add", 
                    new XAttribute("name", field.Name), 
                    new XAttribute("valueType", field.ValueType.AssemblyQualifiedName));

                if (field.Label != null)
                {
                    add.Add(new XAttribute("label", field.Label.Label));
                }

                if (field.PlaceholderText != null)
                {
                    add.Add(new XAttribute("placeholderText", field.PlaceholderText));
                }

                if (!String.IsNullOrEmpty(field.Help))
                {
                    add.Add(new XAttribute("help", field.Help));
                }

                if (field.IsReadOnly)
                {
                    add.Add(new XAttribute("isReadOnly", true));
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

            foreach (var prop in type.GetProperties().Where(p => p.CanRead && p.CanWrite))
            {
                var value = prop.GetValue(instance, null).ToString();

                element.Add(new XAttribute(prop.Name.ToLowerInvariant(), value));
            }
        }

        public static void DeleteModel(DynamicFormDefinition definition)
        {
            var dir = Path.Combine(FormModelsFacade.FormsPath, definition.Name);

            Directory.Delete(dir, true);

            NotifyFormChanges();
        }

        private static void NotifyFormChanges()
        {
            foreach (var action in FormChangeNotifications)
            {
                action();
            }
        }
    }
}
