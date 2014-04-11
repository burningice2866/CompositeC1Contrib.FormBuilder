using System;
using System.Collections.Generic;
using System.Linq;

using CompositeC1Contrib.FormBuilder.Wizard.C1Console.EntityTokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Wizard.C1Console.Workflows
{
    public class AddWizardStepWorkflow : Basic1StepDialogWorkflow
    {
        public AddWizardStepWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Wizard\\AddWizardStepWorkflow.xml") { }

        public static Dictionary<string, string> GetFormNames()
        {
            return FormModelsFacade.GetModels().ToDictionary(m => m.Name, m => m.Name);
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (!BindingExist("StepName"))
            {
                Bindings.Add("StepName", String.Empty);
                Bindings.Add("FormName", String.Empty);
            }
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var wizardToken = (FormWizardEntityToken)EntityToken;
            var stepName = GetBinding<string>("StepName");
            var formName = GetBinding<string>("FormName");

            var wizard = FormWizardsFacade.GetWizard(wizardToken.WizardName);
            var step = new FormWizardStep
            {
                Name = stepName,
                FormName = formName,
                LocalOrdering = wizard.Steps.Count + 1
            };

            wizard.Steps.Add(step);

            FormWizardsFacade.SaveWizard(wizard);

            CreateSpecificTreeRefresher().PostRefreshMesseges(new FormWizardEntityToken(wizard.Name));
            SetSaveStatus(true);
        }

        public override bool Validate()
        {
            var wizardToken = (FormWizardEntityToken)EntityToken;
            var stepName = GetBinding<string>("StepName");

            if (!FormField.IsValidName(stepName))
            {
                ShowFieldMessage("StepName", "Step name is invalid, only a-z and 0-9 is allowed");

                return false;
            }

            var wizard = FormWizardsFacade.GetWizard(wizardToken.WizardName);
            var step = wizard.Steps.SingleOrDefault(s => s.Name == stepName);

            if (step != null)
            {
                ShowFieldMessage("StepName", "Step name already exists");

                return false;
            }

            return true;
        }
    }
}
