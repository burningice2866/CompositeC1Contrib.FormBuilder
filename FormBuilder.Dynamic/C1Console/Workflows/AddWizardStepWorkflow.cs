using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Workflow;
using Composite.Data;

using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Data.Types;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class AddWizardStepWorkflow : Basic1StepDialogWorkflow
    {
        public AddWizardStepWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\AddWizardStepWorkflow.xml") { }

        public static Dictionary<string, string> GetFormNames()
        {
            using (var data = new DataConnection())
            {
                return data.Get<IModelReference>().ToDictionary(f => f.Name, f => f.Name);
            }
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("StepName"))
            {
                return;
            }

            Bindings.Add("StepName", String.Empty);
            Bindings.Add("FormName", String.Empty);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var folderToken = (FormFolderEntityToken)EntityToken;
            var stepName = GetBinding<string>("StepName");
            var formName = GetBinding<string>("FormName");

            var wizard = DynamicWizardsFacade.GetWizard(folderToken.FormName);
            var step = new WizardStepModel
            {
                Name = stepName,
                FormName = formName,
                Label = stepName,
                LocalOrdering = wizard.Model.Steps.Count + 1
            };

            wizard.Model.Steps.Add(step);

            DynamicWizardsFacade.SaveWizard(wizard);

            var wizardStepToken = new FormWizardStepEntityToken(wizard.Name, step.Name);
            var workflowToken = new WorkflowActionToken(typeof(EditWizardStepWorkflow));

            CreateAddNewTreeRefresher(EntityToken).PostRefreshMesseges(wizardStepToken);
            ExecuteAction(wizardStepToken, workflowToken);
        }

        public override bool Validate()
        {
            var stepName = GetBinding<string>("StepName");

            if (!FormFieldModel.IsValidName(stepName))
            {
                ShowFieldMessage("StepName", "Step name is invalid, only a-z and 0-9 is allowed");

                return false;
            }

            var folderToken = (FormFolderEntityToken)EntityToken;
            var wizard = DynamicWizardsFacade.GetWizard(folderToken.FormName);

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
