using System;
using System.Linq;

using CompositeC1Contrib.FormBuilder.Wizard.C1Console.EntityTokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Wizard.C1Console.Workflows
{
    public class EditWizardStepWorkflow : Basic1StepDocumentWorkflow
    {
        public EditWizardStepWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Wizard\\EditWizardStepWorkflow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (!BindingExist("StepName"))
            {
                var wizardStepEntityToken = (FormWizardStepEntityToken) EntityToken;
                var wizard = FormWizardsFacade.GetWizard(wizardStepEntityToken.WizardName);
                var step = wizard.Steps.Single(s => s.Name == wizardStepEntityToken.StepName);

                Bindings.Add("StepName", step.Name);
                Bindings.Add("FormName", step.FormName);
                Bindings.Add("NextButtonLabel", step.NextButtonLabel ?? String.Empty);
                Bindings.Add("PreviousButtonLabel", step.PreviousButtonLabel ?? String.Empty);
            }
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var stepName = GetBinding<string>("StepName");
            var formName = GetBinding<string>("FormName");
            var nextButtonLabel = GetBinding<string>("NextButtonLabel");
            var previousButtonLabel = GetBinding<string>("PreviousButtonLabel");

            var stepToken = (FormWizardStepEntityToken)EntityToken;
            var wizard = FormWizardsFacade.GetWizard(stepToken.WizardName);
            
            var step = wizard.Steps.Single(s => s.Name == stepToken.StepName);

            step.Name = stepName;
            step.FormName = formName;
            step.NextButtonLabel = nextButtonLabel;
            step.PreviousButtonLabel = previousButtonLabel;

            FormWizardsFacade.SaveWizard(wizard);

            CreateSpecificTreeRefresher().PostRefreshMesseges(new FormWizardEntityToken(wizard.Name));
            SetSaveStatus(true);
        }

        public override bool Validate()
        {
            var stepToken = (FormWizardStepEntityToken)EntityToken;
            var stepName = GetBinding<string>("StepName");

            if (stepName != stepToken.StepName)
            {
                if (!FormField.IsValidName(stepName))
                {
                    ShowFieldMessage("StepName", "Step name is invalid, only a-z and 0-9 is allowed");

                    return false;
                }

                var wizard = FormWizardsFacade.GetWizard(stepToken.WizardName);
                var step = wizard.Steps.SingleOrDefault(s => s.Name == stepName);

                if (step != null)
                {
                    ShowFieldMessage("StepName", "Step name already exists");

                    return false;
                }
            }

            return true;
        }
    }
}
