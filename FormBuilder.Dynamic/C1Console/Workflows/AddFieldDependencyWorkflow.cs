using System;
using System.Collections.Generic;
using System.Linq;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class AddFieldDependencyWorkflow : Basic1StepDialogWorkflow
    {
        public AddFieldDependencyWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\AddFieldDependencyWorkflow.xml") { }

        public static Dictionary<string, string> GetFromFields(FieldDependencyEntityToken entityToken)
        {
            var definition = DynamicFormsFacade.GetFormByName(entityToken.FormName);
            var field = definition.Model.Fields.First(f => f.Name == entityToken.FieldName);

            return definition.Model.Fields.Where(f =>
                f.Name != entityToken.FieldName &&
                (f.ValueType == typeof(string) || f.ValueType == typeof(bool) || f.ValueType == typeof(IEnumerable<string>)) &&
                field.DependencyAttributes.All(d => d.ReadFromFieldName != f.Name || d.ReadFromFieldName == entityToken.FromFieldName)
            ).ToDictionary(f => f.Name, f => f.Name);
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("Token"))
            {
                return;
            }

            var token = (FieldDependencyEntityToken)EntityToken;

            Bindings.Add("Token", token);
            Bindings.Add("FromFieldName", String.Empty);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var token = (FieldDependencyEntityToken)EntityToken;
            var definition = DynamicFormsFacade.GetFormByName(token.FormName);

            var field = definition.Model.Fields.Single(f => f.Name == token.FieldName);

            var fromFieldName = GetBinding<string>("FromFieldName");

            var attribute = new DependsOnConstantAttribute(fromFieldName);

            field.Attributes.Add(attribute);

            DynamicFormsFacade.SaveForm(definition);

            CreateSpecificTreeRefresher().PostRefreshMesseges(EntityToken);
        }

        public override bool Validate()
        {
            var token = (FieldDependencyEntityToken)EntityToken;
            var definition = DynamicFormsFacade.GetFormByName(token.FormName);
            var field = definition.Model.Fields.Single(f => f.Name == token.FieldName);

            var fromFieldName = GetBinding<string>("FromFieldName");

            if (String.IsNullOrEmpty(fromFieldName))
            {
                ShowFieldMessage("fromFieldName", "Dependency field not set");

                return false;
            }

            if (field.DependencyAttributes.Any(f => f.ReadFromFieldName == fromFieldName))
            {
                ShowFieldMessage("fromFieldName", "Dependency already exists");

                return false;
            }

            return true;
        }
    }
}
