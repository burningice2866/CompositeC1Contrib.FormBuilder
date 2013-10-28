using System;
using System.Linq;
using System.Workflow.Activities;

using Composite.C1Console.Workflow;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;
using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public sealed partial class EditEmailSubmitHandlerWorkflow : Composite.C1Console.Workflow.Activities.FormsWorkflow
    {
        public EditEmailSubmitHandlerWorkflow()
        {
            InitializeComponent();
        }

        private void initCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            if (!BindingExist("Handler"))
            {
                var token = (FormSubmitHandlerEntityToken)EntityToken;
                var definition = DynamicFormsFacade.GetFormByName(token.FormName);
                var handler = (EmailSubmitHandler)definition.SubmitHandlers.Single(h => h.Name == token.Name);

                Bindings.Add("Handler", handler);
            }
        }

        private void validateSave(object sender, ConditionalEventArgs e)
        {
            var token = (FormSubmitHandlerEntityToken)EntityToken;
            var handler = GetBinding<EmailSubmitHandler>("Handler");

            if (handler.Name != token.Name)
            {
                var definition = DynamicFormsFacade.GetFormByName(token.FormName);

                if (definition.SubmitHandlers.Any(h => h.Name == handler.Name))
                {
                    ShowFieldMessage("Name", "Handler name already exists");

                    e.Result = false;

                    return;
                }
            }

            e.Result = true;
        }

        private void saveCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            var token = (FormSubmitHandlerEntityToken)EntityToken;

            var handler = GetBinding<EmailSubmitHandler>("Handler");
                        
            var definition = DynamicFormsFacade.GetFormByName(token.FormName);
            var existingHandler = definition.SubmitHandlers.Single(h => h.Name == token.Name);

            definition.SubmitHandlers.Remove(existingHandler);
            definition.SubmitHandlers.Add(handler);

            DynamicFormsFacade.SaveForm(definition);

            CreateSpecificTreeRefresher().PostRefreshMesseges(EntityToken);
            SetSaveStatus(true);
        }
    }
}
