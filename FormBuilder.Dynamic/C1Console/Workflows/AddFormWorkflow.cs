using System;

using Composite.C1Console.Workflow;
using Composite.Data;

using CompositeC1Contrib.FormBuilder.Data.Types;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class AddFormWorkflow : BaseAddFormWorkflow
    {
        public AddFormWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\AddFormWorkflow.xml") { }

        public override void OnFinish(object sender, EventArgs e)
        {
            var name = GetBinding<string>("Name");
            var model = new DynamicFormDefinition(name);

            DynamicFormsFacade.SaveForm(model);

            var data = ModelReferenceFacade.GetModelReference(name);
            var token = data.GetDataEntityToken();

            var workflowToken = new WorkflowActionToken(typeof(EditFormWorkflow));

            CreateSpecificTreeRefresher().PostRefreshMesseges(EntityToken);
            ExecuteAction(token, workflowToken);
        }
    }
}
