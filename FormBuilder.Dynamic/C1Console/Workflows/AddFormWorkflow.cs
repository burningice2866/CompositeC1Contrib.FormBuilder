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

            var model = DynamicFormsFacade.GetFormByName(formName);
            if (model != null)
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
            var model = new FormModel(formName);

            DynamicFormsFacade.SaveForm(model);

            var addNewTreeRefresher = CreateAddNewTreeRefresher(EntityToken);
            addNewTreeRefresher.PostRefreshMesseges(new FormInstanceEntityToken(formName));
        }
    }
}
