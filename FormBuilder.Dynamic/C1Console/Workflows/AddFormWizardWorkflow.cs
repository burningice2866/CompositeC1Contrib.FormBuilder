using System;

using Composite.C1Console.Workflow;
using Composite.Data;

using CompositeC1Contrib.FormBuilder.Data.Types;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class AddFormWizardWorkflow : BaseAddFormWorkflow
    {
        public AddFormWizardWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\AddFormWizardWorkflow.xml") { }

        public override void OnFinish(object sender, EventArgs e)
        {
            var name = GetBinding<string>("Name");
            var wizard = new DynamicWizardDefinition(name);

            DynamicWizardsFacade.SaveWizard(wizard);

            var data = ModelReferenceFacade.GetModelReference(name);
            var token = data.GetDataEntityToken();

            var workflowToken = new WorkflowActionToken(typeof(EditFormWizardWorkflow));

            CreateSpecificTreeRefresher().PostRefreshMessages(EntityToken);
            ExecuteAction(token, workflowToken);
        }
    }
}
