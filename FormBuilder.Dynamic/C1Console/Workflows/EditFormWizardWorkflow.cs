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

            var form = (IForm)((DataEntityToken)EntityToken).Data;
            var wizard = DynamicFormWizardsFacade.GetWizard(form.Name);

            SetupFormData(wizard, wizard);

            Bindings.Add("BoundToken", EntityToken);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var wizardToken = GetBinding<DataEntityToken>("BoundToken");
            var form = (IForm)wizardToken.Data;

            var wizard = DynamicFormWizardsFacade.GetWizard(form.Name);

            var requiresCaptcha = GetBinding<bool>("RequiresCaptcha");
            var forceHttpsConnection = GetBinding<bool>("ForceHttpsConnection");

            wizard.RequiresCaptcha = requiresCaptcha;
            wizard.ForceHttps = forceHttpsConnection;

            Save(wizard);
        }
    }
}
