using System;

using Composite.Data;

using CompositeC1Contrib.FormBuilder.Data.Types;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class EditFormWizardWorkflow : BaseEditFormWorkflow
    {
        public EditFormWizardWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\EditFormWizardWorkflow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("BoundToken"))
            {
                return;
            }

            var modelReference = (IModelReference)((DataEntityToken)EntityToken).Data;
            var wizard = DynamicWizardsFacade.GetWizard(modelReference.Name);

            SetupFormData(wizard, wizard.Model);

            Bindings.Add("BoundToken", EntityToken);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var wizardToken = GetBinding<DataEntityToken>("BoundToken");
            var modelReference = (IModelReference)wizardToken.Data;

            var wizard = DynamicWizardsFacade.GetWizard(modelReference.Name);

            var requiresCaptcha = GetBinding<bool>("RequiresCaptcha");
            var forceHttpsConnection = GetBinding<bool>("ForceHttpsConnection");

            wizard.Model.RequiresCaptcha = requiresCaptcha;
            wizard.Model.ForceHttps = forceHttpsConnection;

            Save(wizard);
        }
    }
}
