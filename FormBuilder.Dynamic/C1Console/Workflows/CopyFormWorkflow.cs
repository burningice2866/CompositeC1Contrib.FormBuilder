using System;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class CopyFormWorkflow : AddFormWorkflow
    {
        public override void OnFinish(object sender, EventArgs e)
        {
            var formToken = (FormInstanceEntityToken)EntityToken;
            var formName = GetBinding<string>("FormName");
            var definition = DynamicFormsFacade.GetFormByName(formToken.FormName);

            definition.Copy(formName);

            var treeRefresher = CreateAddNewTreeRefresher(EntityToken);
            treeRefresher.PostRefreshMesseges(new FormInstanceEntityToken(typeof(FormBuilderElementProvider).Name, formName));
        }
    }
}
