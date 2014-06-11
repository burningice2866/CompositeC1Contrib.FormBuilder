using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Composite.C1Console.Actions;
using Composite.C1Console.Forms;
using Composite.C1Console.Forms.DataServices;
using Composite.C1Console.Workflow;
using Composite.Core.Xml;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Configuration;
using CompositeC1Contrib.FormBuilder.Dynamic.Configuration;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public class EditFormWorkflow : Basic1StepDocumentWorkflow
    {
        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("FormName"))
            {
                return;
            }

            var formToken = (FormInstanceEntityToken)EntityToken;
            var definition = DynamicFormsFacade.GetFormByName(formToken.FormName);

            Bindings.Add("FormName", formToken.FormName);
            Bindings.Add("RequiresCaptcha", definition.Model.Attributes.OfType<RequiresCaptchaAttribute>().Any());
            Bindings.Add("ForceHttpsConnection", definition.Model.Attributes.OfType<ForceHttpsConnectionAttribute>().Any());
            Bindings.Add("SubmitButtonLabel", definition.Model.SubmitButtonLabel);
            Bindings.Add("IntroText", definition.IntroText.ToString());
            Bindings.Add("SuccessResponse", definition.SuccessResponse.ToString());

            SetupFormData(definition);
        }

        private void SetupFormData(DynamicFormDefinition definition)
        {
            var markupProvider = new FormDefinitionFileMarkupProvider("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\EditFormWorkflow.xml");
            var formDocument = XDocument.Load(markupProvider.GetReader());

            var root = formDocument.Root;
            if (root == null)
            {
                return;
            }

            var layoutXElement = root.Element(Namespaces.BindingForms10 + FormKeyTagNames.Layout);
            if (layoutXElement == null)
            {
                return;
            }

            var tabPanelElements = layoutXElement.Element(Namespaces.BindingFormsStdUiControls10 + "TabPanels");
            if (tabPanelElements == null)
            {
                return;
            }

            var bindingsXElement = root.Element(Namespaces.BindingForms10 + FormKeyTagNames.Bindings);
            var lastTabElement = tabPanelElements.Elements().Last();

            LoadExtraSettings(definition, bindingsXElement, lastTabElement);

            DeliverFormData("EditForm", StandardUiContainerTypes.Document, formDocument.ToString(), Bindings, BindingsValidationRules);
        }

        private void LoadExtraSettings(DynamicFormDefinition definition, XElement bindingsXElement, XElement lastTabElement)
        {
            var config = FormBuilderConfiguration.GetSection();
            var plugin = (DynamicFormBuilderConfiguration)config.Plugins["dynamic"];

            var settings = plugin.FormSettingsHandler;
            if (settings == null)
            {
                return;
            }

            var formFile = "\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\FunctionExcutorSettings\\" + settings.GetType().FullName + ".xml";
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

            foreach (var prop in settings.GetType().GetProperties())
            {
                var value = prop.GetValue(settings, null);

                Bindings.Add(prop.Name, value);
            }
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var formToken = (FormInstanceEntityToken)EntityToken;
            var definition = DynamicFormsFacade.GetFormByName(formToken.FormName);

            var formName = GetBinding<string>("FormName");
            var submitButtonLabel = GetBinding<string>("SubmitButtonLabel");
            var introText = GetBinding<string>("IntroText");
            var successResponse = GetBinding<string>("SuccessResponse");

            definition.IntroText = XhtmlDocument.Parse(introText);
            definition.SuccessResponse = XhtmlDocument.Parse(successResponse);

            var submitButtonLabelAttr = definition.Model.Attributes.OfType<SubmitButtonLabelAttribute>().SingleOrDefault();
            if (submitButtonLabel != null)
            {
                definition.Model.Attributes.Remove(submitButtonLabelAttr);
            }

            SwitchAttribute<RequiresCaptchaAttribute>("RequiresCaptcha", definition.Model.Attributes);
            SwitchAttribute<ForceHttpsConnectionAttribute>("ForceHttpsConnection", definition.Model.Attributes);

            if (!String.IsNullOrEmpty(submitButtonLabel))
            {
                submitButtonLabelAttr = new SubmitButtonLabelAttribute(submitButtonLabel);
                definition.Model.Attributes.Add(submitButtonLabelAttr);
            }

            SaveExtraSettings(definition);

            if (formName != formToken.FormName)
            {
                DefinitionsFacade.Copy(definition, formName);

                DefinitionsFacade.Delete(definition);
            }
            else
            {
                DynamicFormsFacade.SaveForm(definition);
            }

            CreateSpecificTreeRefresher().PostRefreshMesseges(new FormElementProviderEntityToken());
            SetSaveStatus(true);
        }

        private void SaveExtraSettings(DynamicFormDefinition definition)
        {
            var config = FormBuilderConfiguration.GetSection();
            var plugin = (DynamicFormBuilderConfiguration)config.Plugins["dynamic"];

            var settings = plugin.FormSettingsHandler;
            if (settings == null)
            {
                definition.FormExecutorSettings = null;

                return;
            }

            foreach (var prop in settings.GetType().GetProperties())
            {
                if (!BindingExist(prop.Name))
                {
                    continue;
                }

                var value = GetBinding<object>(prop.Name);

                prop.SetValue(settings, value, null);
            }
        }

        public override bool Validate()
        {
            var formToken = (FormInstanceEntityToken)EntityToken;
            var formName = GetBinding<string>("FormName");

            if (formName == formToken.FormName)
            {
                return true;
            }

            if (!FormModel.IsValidName(formName))
            {
                ShowFieldMessage("FormName", "Form name is invalid, only a-z and 0-9 is allowed");

                return false;
            }

            var isNameInUse = FormModelsFacade.GetModels().Any(m => m.Name == formName);
            if (!isNameInUse)
            {
                return true;
            }

            ShowFieldMessage("FormName", "Form name already exists");

            return false;
        }

        private void SwitchAttribute<T>(string bindingName, IList<Attribute> attributes) where T : Attribute, new()
        {
            var flip = GetBinding<bool>(bindingName);
            var attribute = attributes.OfType<T>().SingleOrDefault();

            if (flip)
            {
                if (attribute == null)
                {
                    attribute = new T();
                    attributes.Add(attribute);
                }
            }
            else
            {
                if (attribute != null)
                {
                    attributes.Remove(attribute);
                }
            }
        }
    }
}
