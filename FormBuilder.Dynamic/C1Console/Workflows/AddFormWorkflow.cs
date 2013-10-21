using System;
using System.Workflow.Activities;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public sealed partial class AddFormWorkflow : Composite.C1Console.Workflow.Activities.FormsWorkflow
    {
        public AddFormWorkflow()
        {
            InitializeComponent();
        }

        private void validateSave(object sender, ConditionalEventArgs e)
        {
            var formName = GetBinding<string>("FormName");

            var definition = DynamicFormsFacade.GetFormByName(formName);
            if (definition != null)
            {
                ShowFieldMessage("Form name", "Form name already exists");

                e.Result = false;

                return;
            }

            e.Result = true;
        }

        private void initCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            if (!BindingExist("FormName"))
            {
                Bindings.Add("FormName", String.Empty);
            }
        }

        private void saveCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            var formName = GetBinding<string>("FormName");
            var model = new DynamicFormDefinition(formName);

            DynamicFormsFacade.SaveForm(model);

            var treeRefresher = CreateAddNewTreeRefresher(EntityToken);
            treeRefresher.PostRefreshMesseges(new FormInstanceEntityToken(formName));
        }
    }
}
