using System;
using System.Collections.Generic;

using Composite.C1Console.Actions;
using Composite.C1Console.Security;
using Composite.Data;

using CompositeC1Contrib.FormBuilder.Data.Types;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions
{
    [ActionExecutor(typeof(DeleteFormActionExecutor))]
    public class DeleteFormActionToken : ActionToken
    {
        private static readonly IEnumerable<PermissionType> _permissionTypes = new[] { PermissionType.Delete };

        public override IEnumerable<PermissionType> PermissionTypes
        {
            get { return _permissionTypes; }
        }

        public override string Serialize()
        {
            return String.Empty;
        }

        public static ActionToken Deserialize(string serializedData)
        {
            return new DeleteFormFieldActionToken();
        }
    }

    public class DeleteFormActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var form = (IForm)((DataEntityToken)entityToken).Data;
            var definition = DefinitionsFacade.GetDefinition(form.Name);

            DefinitionsFacade.Delete(definition);

            new ParentTreeRefresher(flowControllerServicesContainer).PostRefreshMesseges(entityToken);

            return null;
        }
    }
}
