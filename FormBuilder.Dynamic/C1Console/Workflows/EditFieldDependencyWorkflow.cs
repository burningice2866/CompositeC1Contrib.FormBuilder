using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Workflow;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public sealed class EditFieldDependencyWorkflow : Basic1StepDocumentWorkflow
    {
        public EditFieldDependencyWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\EditFieldDependencyWorkflow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("Token"))
            {
                return;
            }

            var token = (FieldDependencyEntityToken)EntityToken;

            Bindings.Add("Token", token);
            Bindings.Add("FromFieldName", token.FromFieldName);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var token = GetBinding<FieldDependencyEntityToken>("Token");
            var fromFieldName = GetBinding<string>("FromFieldName");

            var definition = DynamicFormsFacade.GetFormByName(token.FormName);

            var field = definition.Model.Fields.Get(token.FieldName);
            var dependency = field.DependencyAttributes.Single(d => d.ReadFromFieldName == token.FromFieldName);

            field.Attributes.Remove(dependency);

            field.Attributes.Add(new DependsOnConstantAttribute(fromFieldName, dependency.ResolveRequiredFieldValues()));

            DynamicFormsFacade.SaveForm(definition);

            var newToken = new FieldDependencyEntityToken(token.FormName, token.FieldName, fromFieldName);

            UpdateBinding("Token", newToken);
            SetSaveStatus(true, newToken);
            CreateSpecificTreeRefresher().PostRefreshMessages(new FieldDependencyEntityToken(token.FormName, token.FieldName));
        }
    }
}
