using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Workflow;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Validation;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class AddFieldValidatorWorkflow : Basic1StepDialogWorkflow
    {
        public AddFieldValidatorWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\AddFieldValidatorWorkflow.xml") { }

        public static Dictionary<string, string> GetValidatorTypes()
        {
            var formValidationAttributeType = typeof (FormValidationAttribute);

            var types = CompositionContainerFacade.GetExportedTypes<FormValidationAttribute>(b =>
                b.ForTypesMatching(t => t != formValidationAttributeType
                                        && formValidationAttributeType.IsAssignableFrom(t)
                                        && t.GetConstructor(new[] {typeof (string)}) != null)
                    .Export<FormValidationAttribute>());

            return types.ToDictionary(t => t.AssemblyQualifiedName, t => t.Name);
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("ValidatorType"))
            {
                return;
            }

            Bindings.Add("ValidatorType", String.Empty);
            Bindings.Add("Message", String.Empty);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var token = (FieldValidatorsEntityToken)EntityToken;
            var definition = DynamicFormsFacade.GetFormByName(token.FormName);
            var field = definition.Model.Fields.Single(f => f.Name == token.FieldName);

            var validatorType = Type.GetType(GetBinding<string>("ValidatorType"));
            var message = GetBinding<string>("Message");
            var attribute = (FormValidationAttribute)Activator.CreateInstance(validatorType, new[] { message });

            field.Attributes.Add(attribute);

            DynamicFormsFacade.SaveForm(definition);

            var editToken = new FieldValidatorsEntityToken(token.FormName, token.FieldName, validatorType);
            var workflowToken = new WorkflowActionToken(typeof(EditFieldValidatorWorkflow));

            CreateSpecificTreeRefresher().PostRefreshMesseges(EntityToken);
            ExecuteAction(editToken, workflowToken);
        }

        public override bool Validate()
        {
            var token = (FieldValidatorsEntityToken)EntityToken;
            var definition = DynamicFormsFacade.GetFormByName(token.FormName);

            var field = definition.Model.Fields.Single(f => f.Name == token.FieldName);

            var validatorType = Type.GetType(GetBinding<string>("ValidatorType"));

            if (field.ValidationAttributes.Any(f => f.GetType() == validatorType))
            {
                ShowFieldMessage("ValidatorType", "Validator already exists");

                return false;
            }

            return true;
        }
    }
}
