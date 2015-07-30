using System;

using Composite.Data;

using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Data.Types;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class CopyFormWorkflow : AddFormWorkflow
    {
        public override void OnFinish(object sender, EventArgs e)
        {
            var modelReference = (IModelReference)((DataEntityToken)EntityToken).Data;
            var newName = GetBinding<string>("Name");
            var definition = DefinitionsFacade.GetDefinition(modelReference.Name);

            DefinitionsFacade.Copy(definition, newName);

            CreateSpecificTreeRefresher().PostRefreshMesseges(new FormElementProviderEntityToken());
        }
    }
}
