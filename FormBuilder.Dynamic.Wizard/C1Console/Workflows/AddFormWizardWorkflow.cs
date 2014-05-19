using System;
using System.Linq;

using Composite.C1Console.Workflow;

using CompositeC1Contrib.FormBuilder.Dynamic.Wizard.C1Console.EntityTokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.Wizard.C1Console.Workflows
{
    public class AddFormWizardWorkflow : Basic1StepDialogWorkflow
    {
        public AddFormWizardWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic.Wizard\\AddFormWizardWorkflow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (!BindingExist("WizardName"))
            {
                Bindings.Add("WizardName", String.Empty);
            }
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var wizardName = GetBinding<string>("WizardName");

            var wizard = new DynamicFormWizard
            {
                Name = wizardName
            };

            DynamicFormWizardsFacade.SaveWizard(wizard);

            var treeRefresher = CreateAddNewTreeRefresher(EntityToken);
            treeRefresher.PostRefreshMesseges(new FormWizardEntityToken(wizardName));

            var wizardToken = new FormWizardEntityToken(wizard.Name);
            var workflowToken = new WorkflowActionToken(typeof(EditFormWizardWorkflow));

            CreateAddNewTreeRefresher(EntityToken).PostRefreshMesseges(wizardToken);
            ExecuteAction(wizardToken, workflowToken);
        }

        public override bool Validate()
        {
            var wizardName = GetBinding<string>("WizardName");
            if (!FormModel.IsValidName(wizardName))
            {
                ShowFieldMessage("WizardName", "Wizard name is invalid, only a-z and 0-9 is allowed");

                return false;
            }

            var isNameInUse = DynamicFormWizardsFacade.GetWizards().Any(m => m.Name == wizardName);
            if (!isNameInUse)
            {
                return true;
            }

            ShowFieldMessage("WizardName", "Wizard name already exists");

            return false;
        }
    }
}
