using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Composite.C1Console.Actions;
using Composite.C1Console.Forms;
using Composite.C1Console.Forms.DataServices;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;
using Composite.Core.Xml;
using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Configuration;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;
using CompositeC1Contrib.FormBuilder.Dynamic.Configuration;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public class EditFormFieldWorkflow : Basic1StepDocumentWorkflow
    {
        public override void OnInitialize(object sender, EventArgs e)
        {
            if (!BindingExist("FieldName"))
            {
                var fieldToken = (FormFieldEntityToken)EntityToken;

                var definition = DynamicFormsFacade.GetFormByName(fieldToken.FormName);
                var field = definition.Model.Fields.Single(f => f.Name == fieldToken.FieldName);
                var defaultValue = String.Empty;

                XElement el;
                if (definition.DefaultValues.TryGetValue(field.Name, out el))
                {
                    defaultValue = el.ToString();
                }

                Bindings.Add("FieldName", field.Name);
                Bindings.Add("Label", field.Label == null ? String.Empty : field.Label.Label);
                Bindings.Add("PlaceholderText", field.PlaceholderText);
                Bindings.Add("Help", field.Help);
                Bindings.Add("DefaultValue", defaultValue);
                Bindings.Add("InputElementType", field.InputTypeHandler.GetType().AssemblyQualifiedName);

                Bindings.Add("FieldTypeChangedHandler", new EventHandler(FieldTypeChangedHandler));

                SetupFormData(field);
            }
        }

        private void FieldTypeChangedHandler(object sender, EventArgs e)
        {
            RerenderView();
        }

        private void SetupFormData(FormField field)
        {
            var markupProvider = new FormDefinitionFileMarkupProvider("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\EditFormFieldWorkflow.xml");
            var formDocument = XDocument.Load(markupProvider.GetReader());

            var layoutXElement = formDocument.Root.Element(Namespaces.BindingForms10 + FormKeyTagNames.Layout);
            var bindingsXElement = formDocument.Root.Element(Namespaces.BindingForms10 + FormKeyTagNames.Bindings);
            var tabPanelElements = layoutXElement.Element(Namespaces.BindingFormsStdUiControls10 + "TabPanels");
            var lastTabElement = tabPanelElements.Elements().Last();

            LoadExtraSettings(field, bindingsXElement, lastTabElement);

            DeliverFormData("EditFormField", StandardUiContainerTypes.Document, formDocument.ToString(), Bindings, BindingsValidationRules);
        }

        private void LoadExtraSettings(FormField field, XElement bindingsXElement, XElement lastTabElement)
        {
            var config = FormBuilderConfiguration.GetSection();
            var plugin = (DynamicFormBuilderConfiguration)config.Plugins["dynamic"];
            var inputElement = plugin.InputElementHandlers.Single(el => el.Type == field.InputTypeHandler.GetType());
            var settingsHandler = inputElement.SettingsHandler;

            if (settingsHandler != null)
            {
                var formFile = "\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\InputElementSettings\\" + inputElement.Name + ".xml";
                var settingsMarkupProvider = new FormDefinitionFileMarkupProvider(formFile);
                var formDefinitionElement = XElement.Load(settingsMarkupProvider.GetReader());

                var settingsTab = new XElement(Namespaces.BindingFormsStdUiControls10 + "PlaceHolder");
                var layout = formDefinitionElement.Element(Namespaces.BindingForms10 + FormKeyTagNames.Layout);
                var bindings = formDefinitionElement.Element(Namespaces.BindingForms10 + FormKeyTagNames.Bindings);

                settingsTab.Add(new XAttribute("Label", StringResourceSystemFacade.ParseString(inputElement.Name)));
                settingsTab.Add(layout.Elements());
                bindingsXElement.Add(bindings.Elements());

                lastTabElement.AddAfterSelf(settingsTab);

                var settingsInstance = (InputTypeSettingsHandler)Activator.CreateInstance(settingsHandler);
                settingsInstance.Load(field);

                foreach (var prop in settingsInstance.GetType().GetProperties())
                {
                    var value = prop.GetValue(settingsInstance, null);

                    Bindings.Add(prop.Name, value);
                }
            }
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var fieldToken = (FormFieldEntityToken)EntityToken;

            var fieldName = GetBinding<string>("FieldName");
            var label = GetBinding<string>("Label");
            var placeholderText = GetBinding<string>("PlaceholderText");
            var help = GetBinding<string>("Help");
            var defaultValue = GetBinding<string>("DefaultValue");

            var inputElementType = Type.GetType(GetBinding<string>("InputElementType"));

            var definition = DynamicFormsFacade.GetFormByName(fieldToken.FormName);
            var field = definition.Model.Fields.Single(f => f.Name == fieldToken.FieldName);

            field.Name = fieldName;

            var labelAttr = field.Attributes.OfType<FieldLabelAttribute>().SingleOrDefault();
            if (labelAttr != null)
            {
                field.Attributes.Remove(labelAttr);
            }

            if (!String.IsNullOrEmpty(label))
            {
                labelAttr = new FieldLabelAttribute(label);
                field.Attributes.Add(labelAttr);
            }

            var placeholderAttr = field.Attributes.OfType<PlaceholderTextAttribute>().SingleOrDefault();
            if (placeholderAttr != null)
            {
                field.Attributes.Remove(placeholderAttr);
            }

            if (!String.IsNullOrEmpty(placeholderText))
            {
                placeholderAttr = new PlaceholderTextAttribute(placeholderText);
                field.Attributes.Add(placeholderAttr);
            }

            var helpAttribute = field.Attributes.OfType<FieldHelpAttribute>().FirstOrDefault();
            if (helpAttribute != null)
            {
                field.Attributes.Remove(helpAttribute);
            }

            if (!String.IsNullOrEmpty(help))
            {
                helpAttribute = new FieldHelpAttribute(help);
                field.Attributes.Add(helpAttribute);
            }

            definition.DefaultValues.Remove(field.Name);
            if (!String.IsNullOrEmpty(defaultValue))
            {
                definition.DefaultValues.Add(field.Name, XElement.Parse(defaultValue));
            }

            var inputTypeAttribute = field.Attributes.OfType<InputElementProviderAttribute>().FirstOrDefault();
            if (inputTypeAttribute != null)
            {
                field.Attributes.Remove(inputTypeAttribute);
            }

            inputTypeAttribute = new TypeBasedInputElementProviderAttribute(inputElementType);
            field.Attributes.Add(inputTypeAttribute);

            SaveExtraSettings(field);

            DynamicFormsFacade.SaveForm(definition);

            CreateSpecificTreeRefresher().PostRefreshMesseges(EntityToken);
            SetSaveStatus(true);
        }

        private void SaveExtraSettings(FormField field)
        {
            var config = FormBuilderConfiguration.GetSection();
            var plugin = (DynamicFormBuilderConfiguration)config.Plugins["dynamic"];
            var inputElement = plugin.InputElementHandlers.Single(el => el.Type == field.InputTypeHandler.GetType());
            var settingsHandler = inputElement.SettingsHandler;

            if (settingsHandler != null)
            {
                var settingsInstance = (InputTypeSettingsHandler)Activator.CreateInstance(settingsHandler);
                foreach (var prop in settingsInstance.GetType().GetProperties())
                {
                    var value = GetBinding<object>(prop.Name);

                    prop.SetValue(settingsInstance, value, null);
                }

                settingsInstance.Save(field);
            }
        }

        public override bool Validate()
        {
            var fieldToken = (FormFieldEntityToken)EntityToken;
            var fieldName = GetBinding<string>("FieldName");

            if (fieldName != fieldToken.FieldName)
            {
                if (!FormField.IsValidName(fieldName))
                {
                    ShowFieldMessage("FieldName", "Field name is invalid, only a-z and 0-9 is allowed");

                    return false;
                }

                var definition = DynamicFormsFacade.GetFormByName(fieldToken.FormName);
                var field = definition.Model.Fields.SingleOrDefault(f => f.Name == fieldName);

                if (field != null)
                {
                    ShowFieldMessage("FieldName", "Field name already exists");

                    return false;
                }
            }

            return true;
        }
    }
}
