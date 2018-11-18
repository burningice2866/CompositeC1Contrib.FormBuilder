using System;
using System.Collections.Generic;
using System.Linq;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class AddFieldDependencyValueWorkflow : Basic1StepDialogWorkflow
    {
        public AddFieldDependencyValueWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\AddFieldDependencyValueWorkflow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (!BindingExist("Value"))
            {
                Bindings.Add("Value", String.Empty);
            }
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var token = (FieldDependencyEntityToken)EntityToken;

            var value = GetBinding<string>("Value");

            var definition = DynamicFormsFacade.GetFormByName(token.FormName);

            var field = definition.Model.Fields.Get(token.FieldName);
            var fromField = definition.Model.Fields.Get(token.FromFieldName);
            var dependency = (DependsOnConstantAttribute)field.DependencyAttributes.Single(d => d.ReadFromFieldName == token.FromFieldName);

            var valueType = fromField.ValueType == typeof(IEnumerable<string>) ? typeof(string) : fromField.ValueType;
            var objectValue = Convert.ChangeType(value, valueType);

            dependency.RequiredFieldValues.Add(objectValue);

            DynamicFormsFacade.SaveForm(definition);

            CreateSpecificTreeRefresher().PostRefreshMessages(EntityToken);
        }

        public override bool Validate()
        {
            var token = (FieldDependencyEntityToken)EntityToken;

            var value = GetBinding<string>("Value");

            var definition = DynamicFormsFacade.GetFormByName(token.FormName);
            var field = definition.Model.Fields.Get(token.FieldName);
            var fromField = definition.Model.Fields.Get(token.FromFieldName);
            var dependency = field.DependencyAttributes.Single(d => d.ReadFromFieldName == token.FromFieldName);

            var valueType = fromField.ValueType == typeof(IEnumerable<string>) ? typeof(string) : fromField.ValueType;

            try
            {
                var objectValue = Convert.ChangeType(value, valueType);
            }
            catch (Exception e)
            {
                ShowFieldMessage("Value", e.Message);

                return false;
            }

            if (dependency.ResolveRequiredFieldValues().Any(v => (string)v == value))
            {
                ShowFieldMessage("Value", "Value is not unique");

                return false;
            }

            return true;
        }
    }
}
