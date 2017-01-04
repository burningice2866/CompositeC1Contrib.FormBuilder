using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Composite.C1Console.Users;
using Composite.C1Console.Workflow;
using Composite.Core.Xml;

using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Configuration;
using CompositeC1Contrib.FormBuilder.Dynamic.Configuration;
using CompositeC1Contrib.FormBuilder.Web.UI;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class AddFormFieldWorkflow : Basic1StepDialogWorkflow
    {
        private static readonly Dictionary<string, string> InputElementTypes;

        static AddFormFieldWorkflow()
        {
            var config = FormBuilderConfiguration.GetSection();
            var plugin = (DynamicFormBuilderConfiguration)config.Plugins["dynamic"];

            InputElementTypes = plugin.InputElementHandlers.ToDictionary(e => e.ElementType.GetType().AssemblyQualifiedName, e => e.Name);
        }

        public AddFormFieldWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\AddFormFieldWorkflow.xml") { }

        public static Dictionary<string, string> GetInputElementTypes()
        {
            return InputElementTypes;
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("FieldName"))
            {
                return;
            }

            var folderToken = (FormFolderEntityToken)EntityToken;

            Bindings.Add("HasCustomRenderingLayout", RenderingLayoutFacade.HasCustomRenderingLayout(folderToken.FormName, UserSettings.ActiveLocaleCultureInfo));

            Bindings.Add("FieldName", String.Empty);
            Bindings.Add("InputElementType", InputElementTypes.First().Key);
            Bindings.Add("AddFieldToRenderingLayout", true);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var folderToken = (FormFolderEntityToken)EntityToken;

            var fieldName = GetBinding<string>("FieldName");
            var addFieldToRenderingLayout = GetBinding<bool>("AddFieldToRenderingLayout");
            var definition = DynamicFormsFacade.GetFormByName(folderToken.FormName);
            var field = new FormFieldModel(definition.Model, fieldName, typeof(string), new List<Attribute>());

            var elementType = Type.GetType(GetBinding<string>("InputElementType"));
            var inputTypeAttribute = (InputElementTypeAttribute)Activator.CreateInstance(elementType);

            field.Attributes.Add(inputTypeAttribute);
            definition.Model.Fields.Add(field);

            DynamicFormsFacade.SaveForm(definition);

            if (RenderingLayoutFacade.HasCustomRenderingLayout(folderToken.FormName, UserSettings.ActiveLocaleCultureInfo) && addFieldToRenderingLayout)
            {
                var layut = RenderingLayoutFacade.GetRenderingLayout(folderToken.FormName, UserSettings.ActiveLocaleCultureInfo);

                layut.Body.Add(new XElement(Namespaces.Xhtml + "p", String.Format("%{0}%", fieldName)));

                RenderingLayoutFacade.SaveRenderingLayout(folderToken.FormName, layut, UserSettings.ActiveLocaleCultureInfo);
            }

            var fieldToken = new FormFieldEntityToken(folderToken.FormName, fieldName);
            var workflowToken = new WorkflowActionToken(typeof(EditFormFieldWorkflow));

            CreateAddNewTreeRefresher(EntityToken).PostRefreshMesseges(fieldToken);
            ExecuteAction(fieldToken, workflowToken);
        }

        public override bool Validate()
        {
            var fieldName = GetBinding<string>("FieldName");

            if (!FormFieldModel.IsValidName(fieldName))
            {
                ShowFieldMessage("FieldName", "Field name is invalid, only a-z and 0-9 is allowed");

                return false;
            }

            var folderToken = (FormFolderEntityToken)EntityToken;
            var definition = DynamicFormsFacade.GetFormByName(folderToken.FormName);
            var field = definition.Model.Fields.Get(fieldName);

            if (field != null)
            {
                ShowFieldMessage("FieldName", "Field name already exists");

                return false;
            }

            return true;
        }
    }
}
