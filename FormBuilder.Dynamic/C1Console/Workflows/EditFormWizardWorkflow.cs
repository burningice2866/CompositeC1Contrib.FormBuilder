using System;
using System.Linq;

using Composite.Core.Xml;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class EditFormWizardWorkflow : Basic1StepDocumentWorkflow
    {
        public EditFormWizardWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\EditFormWizardWorkflow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("WizardName"))
            {
                return;
            }

            var formToken = (FormInstanceEntityToken)EntityToken;
            var wizard = DynamicFormWizardsFacade.GetWizard(formToken.FormName);

            Bindings.Add("WizardName", formToken.FormName);
            Bindings.Add("ForceHttpsConnection", wizard.ForceHttpSConnection);
            Bindings.Add("IntroText", wizard.IntroText.ToString());
            Bindings.Add("SuccessResponse", wizard.SuccessResponse.ToString());
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var wizardName = GetBinding<string>("WizardName");
            var forceHttpsConnection = GetBinding<bool>("ForceHttpsConnection");
            var introText = GetBinding<string>("IntroText");
            var successResponse = GetBinding<string>("SuccessResponse");

            var wizardToken = (FormInstanceEntityToken)EntityToken;
            var wizard = DynamicFormWizardsFacade.GetWizard(wizardToken.FormName);

            wizard.Name = wizardName;
            wizard.ForceHttpSConnection = forceHttpsConnection;
            wizard.IntroText = XhtmlDocument.Parse(introText);
            wizard.SuccessResponse = XhtmlDocument.Parse(successResponse);

            DynamicFormWizardsFacade.SaveWizard(wizard);

            CreateSpecificTreeRefresher().PostRefreshMesseges(new FormElementProviderEntityToken());
            SetSaveStatus(true);
        }

        public override bool Validate()
        {
            var wizardToken = (FormInstanceEntityToken)EntityToken;
            var wizardName = GetBinding<string>("WizardName");

            if (wizardName == wizardToken.FormName)
            {
                return true;
            }

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
