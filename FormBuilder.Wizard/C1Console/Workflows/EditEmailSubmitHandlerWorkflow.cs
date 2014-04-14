using System;
using System.Linq;

using Composite.C1Console.Workflow;

using CompositeC1Contrib.FormBuilder.Wizard.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Wizard.SubmitHandlers;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Wizard.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public class EditEmailSubmitHandlerWorkflow : Basic1StepDocumentWorkflow
    {
        public EditEmailSubmitHandlerWorkflow()
            : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Wizard\\EditEmailSubmitHandlerWorkflow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("Handler"))
            {
                return;
            }

            var token = (FormSubmitHandlerEntityToken)EntityToken;
            var wizard = FormWizardsFacade.GetWizard(token.FormName);
            var handler = (EmailSubmitHandler)wizard.SubmitHandlers.Single(h => h.Name == token.Name);

            Bindings.Add("Handler", handler);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var handler = GetBinding<EmailSubmitHandler>("Handler");

            var token = (FormSubmitHandlerEntityToken)EntityToken;
            var wizard = FormWizardsFacade.GetWizard(token.FormName);
            var existingHandler = wizard.SubmitHandlers.Single(h => h.Name == token.Name);
                        
            wizard.SubmitHandlers.Remove(existingHandler);
            wizard.SubmitHandlers.Add(handler);

            FormWizardsFacade.SaveWizard(wizard);

            CreateSpecificTreeRefresher().PostRefreshMesseges(EntityToken);
            SetSaveStatus(true);
        }

        public override bool Validate()
        {
            var token = (FormSubmitHandlerEntityToken)EntityToken;

            var handler = GetBinding<EmailSubmitHandler>("Handler");
            if (handler.Name == token.Name)
            {
                return true;
            }

            var wizard = FormWizardsFacade.GetWizard(token.FormName);
            if (wizard.SubmitHandlers.All(h => h.Name != handler.Name))
            {
                return true;
            }

            ShowFieldMessage("Name", "Handler name already exists");

            return false;
        }
    }
}
