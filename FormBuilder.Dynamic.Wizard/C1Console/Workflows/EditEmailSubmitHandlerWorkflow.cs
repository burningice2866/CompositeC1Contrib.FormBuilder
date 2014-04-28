using System.Linq;

using Composite.C1Console.Workflow;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;
using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;
using CompositeC1Contrib.FormBuilder.Dynamic.Wizard.C1Console.EntityTokens;

namespace CompositeC1Contrib.FormBuilder.Dynamic.Wizard.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public class EditEmailSubmitHandlerWorkflow : BaseEditEmailSubmitHandlerWorkflow<FormSubmitHandlerEntityToken>
    {
        public EditEmailSubmitHandlerWorkflow() : base("CompositeC1Contrib.FormBuilder.Dynamic.Wizard") { }

        protected override BaseDynamicEmailSubmitHandler GetHandler()
        {
            var wizard = DynamicFormWizardsFacade.GetWizard(SubmitHandlerEntityToken.FormName);

            return (BaseDynamicEmailSubmitHandler)wizard.SubmitHandlers.Single(h => h.Name == SubmitHandlerEntityToken.Name);
        }

        protected override IDynamicFormDefinition GetDefinition()
        {
            return DynamicFormWizardsFacade.GetWizard(SubmitHandlerEntityToken.FormName);
        }

        protected override void SaveDefintion(IDynamicFormDefinition definition)
        {
            DynamicFormWizardsFacade.SaveWizard((DynamicFormWizard)definition);
        }
    }
}
