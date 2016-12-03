using System;
using System.Linq;
using System.Xml.Linq;

using Composite.C1Console.Actions;
using Composite.C1Console.Forms;
using Composite.C1Console.Forms.DataServices;
using Composite.C1Console.Users;
using Composite.Core.Xml;
using Composite.Data;

using CompositeC1Contrib.FormBuilder.Configuration;
using CompositeC1Contrib.FormBuilder.Data.Types;
using CompositeC1Contrib.FormBuilder.Dynamic.Configuration;
using CompositeC1Contrib.Localization;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public abstract class BaseEditFormWorkflow : Basic1StepDocumentWorkflow
    {
        private readonly string _formFile;

        protected BaseEditFormWorkflow(string formFile)
        {
            _formFile = formFile;
        }

        protected void SetupFormData(IDynamicDefinition definition, IModel model)
        {
            Bindings.Add("Name", definition.Name);

            Bindings.Add("RequiresCaptcha", model.RequiresCaptcha);
            Bindings.Add("ForceHttpsConnection", model.ForceHttps);

            Bindings.Add("IntroText", GetValue("IntroText"));
            Bindings.Add("SuccessResponse", GetValue("SuccessResponse"));

            var markupProvider = new FormDefinitionFileMarkupProvider(_formFile);
            var formDocument = XDocument.Load(markupProvider.GetReader());

            var root = formDocument.Root;

            var tabPanelElements = root?.Element(Namespaces.BindingForms10 + FormKeyTagNames.Layout)?.Element(Namespaces.BindingFormsStdUiControls10 + "TabPanels");
            if (tabPanelElements == null)
            {
                return;
            }

            var bindingsXElement = root.Element(Namespaces.BindingForms10 + FormKeyTagNames.Bindings);
            var lastTabElement = tabPanelElements.Elements().Last();

            LoadExtraSettings(definition, bindingsXElement, lastTabElement);

            DeliverFormData("EditForm", StandardUiContainerTypes.Document, formDocument.ToString(), Bindings, BindingsValidationRules);
        }

        private void LoadExtraSettings(IDynamicDefinition definition, XElement bindingsXElement, XElement lastTabElement)
        {
            var config = FormBuilderConfiguration.GetSection();
            var plugin = (DynamicFormBuilderConfiguration)config.Plugins["dynamic"];

            var settingsType = plugin.SettingsHandler;
            if (settingsType == null)
            {
                return;
            }

            var formFile = "\\InstalledPackages\\" + settingsType.Namespace + "\\" + settingsType.Name + ".xml";
            var settingsMarkupProvider = new FormDefinitionFileMarkupProvider(formFile);
            var formDefinitionElement = XElement.Load(settingsMarkupProvider.GetReader());
            var settingsTab = new XElement(Namespaces.BindingFormsStdUiControls10 + "PlaceHolder");

            var layout = formDefinitionElement.Element(Namespaces.BindingForms10 + FormKeyTagNames.Layout);
            if (layout == null)
            {
                return;
            }

            var bindings = formDefinitionElement.Element(Namespaces.BindingForms10 + FormKeyTagNames.Bindings);
            if (bindings == null)
            {
                return;
            }

            settingsTab.Add(new XAttribute("Label", "Extra Settings"));
            settingsTab.Add(layout.Elements());

            bindingsXElement.Add(bindings.Elements());
            lastTabElement.AddAfterSelf(settingsTab);

            var settings = definition.Settings ?? (IFormSettings)Activator.CreateInstance(settingsType);

            foreach (var prop in settingsType.GetProperties())
            {
                var value = prop.GetValue(settings, null);

                Bindings.Add(prop.Name, value);
            }
        }

        public override bool Validate()
        {
            var token = GetBinding<DataEntityToken>("BoundToken");
            var modelReference = (IModelReference)token.Data;

            var name = GetBinding<string>("Name");

            if (name == modelReference.Name)
            {
                return true;
            }

            if (!FormModel.IsValidName(name))
            {
                ShowFieldMessage("Name", "Form name is invalid, only a-z and 0-9 is allowed");

                return false;
            }

            var isNameInUse = DefinitionsFacade.GetDefinitions().Any(m => m.Name == name);
            if (!isNameInUse)
            {
                return true;
            }

            ShowFieldMessage("Name", "Form name already exists");

            return false;
        }

        protected void Save(IDynamicDefinition definition)
        {
            SaveExtraSettings(definition);

            var token = GetBinding<DataEntityToken>("BoundToken");
            var modelReference = (IModelReference)token.Data;

            var name = GetBinding<string>("Name");

            var introText = GetBinding<string>("IntroText");
            var successResponse = GetBinding<string>("SuccessResponse");

            using (var writer = ResourceFacade.GetResourceWriter(UserSettings.ActiveLocaleCultureInfo))
            {
                writer.AddResource(GetKey("IntroText"), introText);
                writer.AddResource(GetKey("SuccessResponse"), successResponse);
            }

            var isNewName = name != modelReference.Name;
            if (isNewName)
            {
                LocalizationsFacade.RenameNamespace("Forms." + modelReference.Name, "Forms." + name, "FormBuilder");

                DefinitionsFacade.Copy(definition, name);
                DefinitionsFacade.Delete(definition);

                modelReference = ModelReferenceFacade.GetModelReference(name);
                token = modelReference.GetDataEntityToken();

                UpdateBinding("BoundToken", token);
                SetSaveStatus(true, token);
            }
            else
            {
                DefinitionsFacade.Save(definition);

                SetSaveStatus(true);
            }

            CreateParentTreeRefresher().PostRefreshMesseges(EntityToken);
        }

        private void SaveExtraSettings(IDynamicDefinition definition)
        {
            var config = FormBuilderConfiguration.GetSection();
            var plugin = (DynamicFormBuilderConfiguration)config.Plugins["dynamic"];

            var settingsType = plugin.SettingsHandler;
            if (settingsType == null)
            {
                definition.Settings = null;

                return;
            }

            definition.Settings = (IFormSettings)Activator.CreateInstance(settingsType);

            foreach (var prop in settingsType.GetProperties().Where(p => BindingExist(p.Name)))
            {
                var value = GetBinding<object>(prop.Name);

                prop.SetValue(definition.Settings, value, null);
            }
        }

        protected string GetValue(string setting)
        {
            var key = GetKey(setting);

            return Localization.T(key, UserSettings.ActiveLocaleCultureInfo);
        }

        protected string GetKey(string setting)
        {
            var formToken = GetBinding<DataEntityToken>("BoundToken");
            var modelReference = (IModelReference)formToken.Data;

            return "Forms." + modelReference.Name + "." + setting;
        }
    }
}
