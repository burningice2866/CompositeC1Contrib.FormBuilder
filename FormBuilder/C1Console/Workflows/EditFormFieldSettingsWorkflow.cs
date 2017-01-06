using System;
using System.Linq;
using System.Xml.Linq;

using Composite.C1Console.Actions;
using Composite.C1Console.Forms;
using Composite.C1Console.Forms.DataServices;
using Composite.Core.Xml;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Validation;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.C1Console.Workflows
{
    public class EditFormFieldSettingsWorkflow : Basic1StepDocumentWorkflow
    {
        private const string XmlFilePath = "\\InstalledPackages\\CompositeC1Contrib.FormBuilder\\EditFormFieldSettingsWorkflow.xml";

        public EditFormFieldSettingsWorkflow() : base(null) { }

        private FormFieldEntityToken FieldEntityToken => (FormFieldEntityToken)EntityToken;

        private string GetValue(string setting)
        {
            var key = GetKey(setting);

            return Localization.T(key);
        }

        private string GetKey(string setting)
        {
            setting = FieldEntityToken.FieldName + "." + setting;

            return Localization.GenerateKey(FieldEntityToken.FormName, setting);
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("Label"))
            {
                return;
            }

            var model = ModelsFacade.GetModel<FormModel>(FieldEntityToken.FormName);
            var field = model.Fields.Get(FieldEntityToken.FieldName);

            Bindings.Add("Label", GetValue("Label") ?? field.Label);

            SetupFormXml();
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var label = GetBinding<string>("Label");

            using (var writer = ResourceFacade.GetResourceWriter())
            {
                writer.AddResource(GetKey("Label"), label);

                var model = ModelsFacade.GetModel<FormModel>(FieldEntityToken.FormName);
                var field = model.Fields.Get(FieldEntityToken.FieldName);

                if (field.Attributes.OfType<PlaceholderTextAttribute>().Any())
                {
                    var placeholderText = GetBinding<string>("PlaceholderText");

                    writer.AddResource(GetKey("PlaceholderText"), placeholderText);
                }

                if (field.Attributes.OfType<FieldHelpAttribute>().Any())
                {
                    var help = GetBinding<string>("Help");

                    writer.AddResource(GetKey("Help"), help);
                }

                var validationAttributes = field.Attributes.OfType<FormValidationAttribute>();
                foreach (var attr in validationAttributes)
                {
                    var name = attr.GetType().Name;

                    var key = "Validation." + name;
                    var binding = "Validation-" + name;

                    var value = GetBinding<string>(binding);

                    writer.AddResource(GetKey(key), value);
                }
            }

            var treeRefresher = CreateParentTreeRefresher();
            treeRefresher.PostRefreshMesseges(EntityToken);

            SetSaveStatus(true);
        }

        private void SetupFormXml()
        {
            XDocument formDocument;

            var markupProvider = new FormDefinitionFileMarkupProvider(XmlFilePath);
            using (var reader = markupProvider.GetReader())
            {
                formDocument = XDocument.Load(reader);
            }

            if (formDocument.Root == null)
            {
                return;
            }

            var model = ModelsFacade.GetModel<FormModel>(FieldEntityToken.FormName);
            var field = model.Fields.Get(FieldEntityToken.FieldName);

            var bindingsXElement = formDocument.Root.Element(Namespaces.BindingForms10 + FormKeyTagNames.Bindings);
            var layoutElement = formDocument.Root.Element(Namespaces.BindingForms10 + FormKeyTagNames.Layout);

            if (bindingsXElement == null || layoutElement == null)
            {
                return;
            }

            var tabPanelsElement = layoutElement.Element(Namespaces.BindingFormsStdUiControls10 + "TabPanels");
            if (tabPanelsElement == null)
            {
                return;
            }

            var basicFieldGroup = tabPanelsElement.Element(Namespaces.BindingFormsStdUiControls10 + "PlaceHolder").Element(Namespaces.BindingFormsStdUiControls10 + "FieldGroup");

            if (field.Attributes.OfType<PlaceholderTextAttribute>().Any())
            {
                var binding = "PlaceholderText";

                AddDynamicBinding(bindingsXElement, binding, GetValue(binding) ?? field.PlaceholderText);
                AddTextBox(basicFieldGroup, "Placeholder text", binding);
            }

            if (field.Attributes.OfType<FieldHelpAttribute>().Any())
            {
                var binding = "Help";

                AddDynamicBinding(bindingsXElement, binding, GetValue(binding) ?? field.Help);
                AddTextBox(basicFieldGroup, "Help", binding);
            }

            var validationAttributes = field.Attributes.OfType<FormValidationAttribute>();
            if (!validationAttributes.Any())
            {
                DeliverFormData(FieldEntityToken.FieldName, StandardUiContainerTypes.Document, formDocument.ToString(), Bindings, BindingsValidationRules);

                return;
            }

            basicFieldGroup.Attribute("Label").Remove();
            tabPanelsElement.Element(Namespaces.BindingFormsStdUiControls10 + "PlaceHolder").Add(new XAttribute("Label", "Basic"));

            var fieldGroup = new XElement(Namespaces.BindingFormsStdUiControls10 + "FieldGroup");

            foreach (var attr in validationAttributes)
            {
                var name = attr.GetType().Name;
                var label = attr.AttributeName();
                var binding = "Validation-" + name;

                AddTextBox(fieldGroup, label, binding);
                AddDynamicBinding(bindingsXElement, binding, GetValue("Validation." + name) ?? attr.GetValidationMessage(field));
            }

            tabPanelsElement.Add(
                new XElement(Namespaces.BindingFormsStdUiControls10 + "PlaceHolder", new XAttribute("Label", "Validation"),
                    fieldGroup
                )
            );

            DeliverFormData(FieldEntityToken.FieldName, StandardUiContainerTypes.Document, formDocument.ToString(), Bindings, BindingsValidationRules);
        }

        private static void AddTextBox(XElement fieldGroup, string label, string binding)
        {
            fieldGroup.Add(
                new XElement(Namespaces.BindingFormsStdUiControls10 + "TextBox", new XAttribute("Label", label),
                    new XElement(Namespaces.BindingFormsStdUiControls10 + "TextBox.Text",
                        new XElement(Namespaces.BindingForms10 + FormKeyTagNames.Bind, new XAttribute("source", binding))
                    )
                )
            );
        }

        private void AddDynamicBinding(XElement bindingsXElement, string name, string value)
        {
            bindingsXElement.Add(new XElement(Namespaces.BindingForms10 + FormKeyTagNames.Binding,
                                new XAttribute("name", name),
                                new XAttribute("type", typeof(string).FullName),
                                new XAttribute("optional", "true")
                                ));

            Bindings.Add(name, value);
        }
    }
}