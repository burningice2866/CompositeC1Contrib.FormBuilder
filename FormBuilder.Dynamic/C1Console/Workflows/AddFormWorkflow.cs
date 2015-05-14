using System;
using System.Linq;

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

            var data = FormDataFacade.GetFormData(name);
            var token = data.GetDataEntityToken();

            var workflowToken = new WorkflowActionToken(typeof(EditFormWorkflow));

            CreateSpecificTreeRefresher().PostRefreshMesseges(EntityToken);
            ExecuteAction(token, workflowToken);
        }

        public override bool Validate()
        {
            var name = GetBinding<string>("Name");

            var valid = base.Validate();
            if (!valid)
            {
                return false;
            }

            var isNameInUse = DefinitionsFacade.GetDefinitions().Any(m => m.Name == name);
            if (isNameInUse)
            {
                ShowFieldMessage("Name", "Form name already exists");

                return false;
            }

            return true;
        }
    }
}
