using System;
using System.Collections.Generic;
using System.Linq;
using System.Workflow.Activities;

using Composite.C1Console.Workflow;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public sealed partial class AddFormFieldWorkflow : Composite.C1Console.Workflow.Activities.FormsWorkflow
    {
        public AddFormFieldWorkflow()
        {
            InitializeComponent();
        }

        private void validateSave(object sender, ConditionalEventArgs e)
        {
            var folderToken = (FormFolderEntityToken)EntityToken;

            var fieldName = GetBinding<string>("FieldName");
            var definition = DynamicFormsFacade.GetFormByName(folderToken.FormName);
            var field = definition.Model.Fields.SingleOrDefault(f => f.Name == fieldName);

            if (field != null)
            {
                ShowFieldMessage("Field name", "Field name already exists");

                e.Result = false;

                return;
            }


            e.Result = true;
        }

        private void initCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            if (!BindingExist("FieldName"))
            {
                Bindings.Add("FieldName", String.Empty);
            }
        }

        private void saveCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            var folderToken = (FormFolderEntityToken)EntityToken;

            var fieldName = GetBinding<string>("FieldName");
            var definition = DynamicFormsFacade.GetFormByName(folderToken.FormName);
            var field = new FormField(definition.Model, fieldName, typeof(string), new List<Attribute>());

            definition.Model.Fields.Add(field);

            DynamicFormsFacade.SaveForm(definition);

            var fieldToken = new FormFieldEntityToken(folderToken.FormName, fieldName);
            var workflowToken = new WorkflowActionToken(typeof(EditFormFieldWorkflow));
            var treeRefresher = CreateAddNewTreeRefresher(EntityToken);

            treeRefresher.PostRefreshMesseges(fieldToken);
            ExecuteAction(fieldToken, workflowToken);
        }
    }
}
