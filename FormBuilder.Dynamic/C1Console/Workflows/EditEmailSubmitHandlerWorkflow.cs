using System.Linq;

using Composite.C1Console.Workflow;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public class EditEmailSubmitHandlerWorkflow : BaseEditEmailSubmitHandlerWorkflow<FormSubmitHandlerEntityToken>
    {
        public EditEmailSubmitHandlerWorkflow() : base("CompositeC1Contrib.FormBuilder.Dynamic") { }

        protected override BaseDynamicEmailSubmitHandler GetHandler()
        {
            var definition = DynamicFormsFacade.GetFormByName(SubmitHandlerEntityToken.FormName);

            return (BaseDynamicEmailSubmitHandler)definition.SubmitHandlers.Single(h => h.Name == SubmitHandlerEntityToken.Name);
        }

        protected override IDynamicFormDefinition GetDefinition()
        {
            return DynamicFormsFacade.GetFormByName(SubmitHandlerEntityToken.FormName);
        }

        protected override void SaveDefintion(IDynamicFormDefinition definition)
        {
            DynamicFormsFacade.SaveForm((DynamicFormDefinition)definition);
        }
    }
}
