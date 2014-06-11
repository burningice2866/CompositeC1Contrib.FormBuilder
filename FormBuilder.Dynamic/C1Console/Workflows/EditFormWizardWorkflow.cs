using System;

using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class EditFormWizardWorkflow : BaseEditFormWorkflow
    {
        public EditFormWizardWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\EditFormWizardWorkflow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("Name"))
            {
                return;
            }

            var formToken = (FormInstanceEntityToken)EntityToken;
            var wizard = DynamicFormWizardsFacade.GetWizard(formToken.FormName);

            Bindings.Add("ForceHttpsConnection", wizard.ForceHttpSConnection);

            SetupFormData(wizard);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var wizardToken = (FormInstanceEntityToken)EntityToken;
            var wizard = DynamicFormWizardsFacade.GetWizard(wizardToken.FormName);

            var forceHttpsConnection = GetBinding<bool>("ForceHttpsConnection");

            wizard.ForceHttpSConnection = forceHttpsConnection;

            Save(wizard);
        }
    }
}
