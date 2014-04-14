using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Actions;
using Composite.C1Console.Security;

using CompositeC1Contrib.FormBuilder.Wizard.C1Console.EntityTokens;

namespace CompositeC1Contrib.FormBuilder.Wizard.C1Console.Actions
{
    [ActionExecutor(typeof(DeleteSubmitHandlerActionExecutor))]
    public class DeleteSubmitHandlerActionToken : ActionToken
    {
        private static readonly IEnumerable<PermissionType> _permissionTypes = new[] { PermissionType.Administrate };

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
            return new DeleteSubmitHandlerActionToken();
        }
    }

    public class DeleteSubmitHandlerActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var token = (FormSubmitHandlerEntityToken)entityToken;
            var wizard = FormWizardsFacade.GetWizard(token.FormName);
            var handler = wizard.SubmitHandlers.Single(h => h.Name == token.Name);

            wizard.SubmitHandlers.Remove(handler);

            FormWizardsFacade.SaveWizard(wizard);

            new ParentTreeRefresher(flowControllerServicesContainer).PostRefreshMesseges(entityToken);

            return null;
        }
    }
}
