using System;
using System.Linq;

using CompositeC1Contrib.FormBuilder.Wizard.C1Console.EntityTokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Wizard.C1Console.Workflows
{
    public class EditFormWizardWorkflow : Basic1StepDialogWorkflow
    {
        public EditFormWizardWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Wizard\\EditFormWizardWorkflow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (!BindingExist("WizardName"))
            {
                var wizardToken = (FormWizardEntityToken)EntityToken;

                Bindings.Add("WizardName", wizardToken.WizardName);
            }
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var wizardName = GetBinding<string>("WizardName");
            var wizardToken = (FormWizardEntityToken)EntityToken;
            var wizard = FormWizardsFacade.GetWizard(wizardToken.WizardName);

            wizard.Name = wizardName;

            FormWizardsFacade.SaveWizard(wizard);

            CreateSpecificTreeRefresher().PostRefreshMesseges(new FormWizardsElementProviderEntityToken());
            SetSaveStatus(true);
        }

        public override bool Validate()
        {
            var wizardToken = (FormWizardEntityToken)EntityToken;
            var wizardName = GetBinding<string>("WizardName");

            if (wizardName != wizardToken.WizardName)
            {
                if (!FormModel.IsValidName(wizardName))
                {
                    ShowFieldMessage("WizardName", "Wizard name is invalid, only a-z and 0-9 is allowed");

                    return false;
                }

                var isNameInUse = FormWizardsFacade.GetWizards().Any(m => m.Name == wizardName);
                if (isNameInUse)
                {
                    ShowFieldMessage("WizardName", "Wizard name already exists");

                    return false;
                }
            }

            return true;
        }
    }
}
