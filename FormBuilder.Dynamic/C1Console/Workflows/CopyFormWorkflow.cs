using System;
using System.Workflow.Activities;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class CopyFormWorkflow : AddFormWorkflow
    {
        public override void OnSave(object sender, EventArgs e)
        {
            var formToken = (FormInstanceEntityToken)EntityToken;

            var formName = GetBinding<string>("FormName");

            var definition = DynamicFormsFacade.CopyFormByName(formToken.FormName, formName);

            DynamicFormsFacade.SaveForm(definition);

            var treeRefresher = CreateAddNewTreeRefresher(EntityToken);
            treeRefresher.PostRefreshMesseges(new FormInstanceEntityToken(typeof(FormBuilderElementProvider).Name, formName));
        }
    }
}
