using System;
using System.Linq;
using Composite.C1Console.Workflow;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public sealed class EditFieldValidatorWorkflow : Basic1StepDocumentWorkflow
    {
        public EditFieldValidatorWorkflow()
            : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\EditFieldValidatorWorkflow.xml")
        {
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (!BindingExist("Message"))
            {
                var token = (FieldValidatorsEntityToken)EntityToken;

                var definition = DynamicFormsFacade.GetFormByName(token.FormName);
                var field = definition.Model.Fields.Single(f => f.Name == token.FieldName);
                var validator = field.ValidationAttributes.Single(v => v.GetType().AssemblyQualifiedName == token.Type);

                Bindings.Add("FieldName", token.FieldName);
                Bindings.Add("Message", validator.Message);
            }
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var token = (FieldValidatorsEntityToken)EntityToken;

            var message = GetBinding<string>("Message");

            var definition = DynamicFormsFacade.GetFormByName(token.FormName);
            var field = definition.Model.Fields.Single(f => f.Name == token.FieldName);
            var validator = field.ValidationAttributes.Single(v => v.GetType().AssemblyQualifiedName == token.Type);

            validator.Message = message;

            DynamicFormsFacade.SaveForm(definition);

            CreateSpecificTreeRefresher().PostRefreshMesseges(EntityToken);
            SetSaveStatus(true);
        }
    }
}
