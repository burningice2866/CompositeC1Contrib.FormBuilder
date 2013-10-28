using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Actions;
using Composite.C1Console.Security;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions
{
    [ActionExecutor(typeof(DeleteFormActionExecutor))]
    public class DeleteFormActionToken : ActionToken
    {
        private static IEnumerable<PermissionType> _permissionTypes = new PermissionType[] { PermissionType.Administrate };

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
            var fieldToken = (FormInstanceEntityToken)entityToken;
            var definition = DynamicFormsFacade.GetFormByName(fieldToken.FormName);

            DynamicFormsFacade.DeleteModel(definition);

            new ParentTreeRefresher(flowControllerServicesContainer).PostRefreshMesseges(entityToken);

            return null;
        }
    }
}
