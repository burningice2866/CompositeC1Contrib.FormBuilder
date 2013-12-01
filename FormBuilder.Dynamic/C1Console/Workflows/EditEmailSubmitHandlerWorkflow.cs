using System;
using System.Linq;
using Composite.C1Console.Workflow;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;
using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public class EditEmailSubmitHandlerWorkflow : Basic1StepDocumentWorkflow
    {
        public EditEmailSubmitHandlerWorkflow()
            : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\EditEmailSubmitHandlerWorkflow.xml")
        {
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (!BindingExist("Handler"))
            {
                var token = (FormSubmitHandlerEntityToken)EntityToken;
                var definition = DynamicFormsFacade.GetFormByName(token.FormName);
                var handler = (EmailSubmitHandler)definition.SubmitHandlers.Single(h => h.Name == token.Name);

                Bindings.Add("Handler", handler);
            }
        }

        public override void OnFinish(object sender, EventArgs e)
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

        public override bool Validate()
        {
            var token = (FormSubmitHandlerEntityToken)EntityToken;
            var handler = GetBinding<EmailSubmitHandler>("Handler");

            if (handler.Name != token.Name)
            {
                var definition = DynamicFormsFacade.GetFormByName(token.FormName);

                if (definition.SubmitHandlers.Any(h => h.Name == handler.Name))
                {
                    ShowFieldMessage("Name", "Handler name already exists");

                    return false;
                }
            }

            return true;
        }
    }
}
