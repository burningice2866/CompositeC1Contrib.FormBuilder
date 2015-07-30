using System;
using System.Linq;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class EditWizardStepWorkflow : Basic1StepDocumentWorkflow
    {
        public EditWizardStepWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\EditWizardStepWorkflow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("BoundToken"))
            {
                return;
            }

            var wizardStepEntityToken = (FormWizardStepEntityToken)EntityToken;
            var wizard = DynamicWizardsFacade.GetWizard(wizardStepEntityToken.WizardName);
            var step = wizard.Model.Steps.Single(s => s.Name == wizardStepEntityToken.StepName);

            Bindings.Add("StepName", step.Name);
            Bindings.Add("FormName", step.FormName);
            Bindings.Add("StepLabel", step.Label);
            Bindings.Add("NextButtonLabel", step.NextButtonLabel ?? String.Empty);
            Bindings.Add("PreviousButtonLabel", step.PreviousButtonLabel ?? String.Empty);

            Bindings.Add("BoundToken", wizardStepEntityToken);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var stepToken = GetBinding<FormWizardStepEntityToken>("BoundToken");

            var stepName = GetBinding<string>("StepName");
            var formName = GetBinding<string>("FormName");
            var stepLabel = GetBinding<string>("StepLabel");
            var nextButtonLabel = GetBinding<string>("NextButtonLabel");
            var previousButtonLabel = GetBinding<string>("PreviousButtonLabel");

            var isNewName = stepName != stepToken.StepName;

            var wizard = DynamicWizardsFacade.GetWizard(stepToken.WizardName);

            var step = wizard.Model.Steps.Single(s => s.Name == stepToken.StepName);

            step.Name = stepName;
            step.FormName = formName;
            step.Label = stepLabel;
            step.NextButtonLabel = nextButtonLabel;
            step.PreviousButtonLabel = previousButtonLabel;

            DynamicWizardsFacade.SaveWizard(wizard);

            if (isNewName)
            {
                stepToken = new FormWizardStepEntityToken(wizard.Name, stepName);

                UpdateBinding("BoundToken", stepToken);
                SetSaveStatus(true, stepToken);
            }
            else
            {
                SetSaveStatus(true);
            }

            CreateParentTreeRefresher().PostRefreshMesseges(EntityToken);
        }

        public override bool Validate()
        {
            var stepToken = GetBinding<FormWizardStepEntityToken>("BoundToken");
            var stepName = GetBinding<string>("StepName");

            if (stepName == stepToken.StepName)
            {
                return true;
            }

            if (!FormFieldModel.IsValidName(stepName))
            {
                ShowFieldMessage("StepName", "Step name is invalid, only a-z and 0-9 is allowed");

                return false;
            }

            var wizard = DynamicWizardsFacade.GetWizard(stepToken.WizardName);

            var step = wizard.Model.Steps.SingleOrDefault(s => s.Name == stepName);
            if (step == null)
            {
                return true;
            }

            ShowFieldMessage("StepName", "Step name already exists");

            return false;
        }
    }
}
