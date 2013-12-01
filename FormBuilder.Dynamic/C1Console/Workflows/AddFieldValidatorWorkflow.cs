using System;
using System.Collections.Generic;
using System.Linq;
using Composite.C1Console.Workflow;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;
using CompositeC1Contrib.FormBuilder.Validation;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class AddFieldValidatorWorkflow : Basic1StepDialogWorkflow
    {
        public AddFieldValidatorWorkflow()
            : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\AddFieldValidatorWorkflow.xml")
        {
        }

        public static Dictionary<string, string> GetValidatorTypes()
        {
            var returnList = new List<Type>();
            var formAttributeType = typeof(FormValidationAttribute);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in assemblies)
            {
                try
                {
                    var types = asm.GetTypes()
                        .Where(t => t != formAttributeType)
                        .Where(t => formAttributeType.IsAssignableFrom(t))
                        .Where(t => t.GetConstructor(new[] { typeof(string) }) != null);
                    ;

                    returnList.AddRange(types);
                }
                catch { }
            }

            return returnList.ToDictionary(t => t.AssemblyQualifiedName, t => t.Name); ;
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (!BindingExist("ValidatorType"))
            {
                Bindings.Add("ValidatorType", String.Empty);
                Bindings.Add("Message", String.Empty);
            }
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
    }
}
