using System;

using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class CopyFormWorkflow : AddFormWorkflow
    {
        public override void OnFinish(object sender, EventArgs e)
        {
            var formToken = (FormInstanceEntityToken)EntityToken;
            var formName = GetBinding<string>("FormName");
            var definition = DefinitionsFacade.GetDefinition(formToken.FormName);

            DefinitionsFacade.Copy(definition, formName);

            CreateAddNewTreeRefresher(EntityToken).PostRefreshMesseges(new FormInstanceEntityToken(typeof(FormBuilderElementProvider).Name, formName));
        }
    }
}
