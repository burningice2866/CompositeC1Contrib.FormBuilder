using System;
using System.Collections.Generic;

using Composite.C1Console.Actions;
using Composite.C1Console.Security;

using CompositeC1Contrib.FormBuilder.Dynamic.Wizard.C1Console.EntityTokens;

namespace CompositeC1Contrib.FormBuilder.Dynamic.Wizard.C1Console.Actions
{
    [ActionExecutor(typeof(DeleteFormWizardActionExecutor))]
    public class DeleteFormWizardActionToken : ActionToken
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
            return new DeleteFormWizardActionToken();
        }
    }

    public class DeleteFormWizardActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var wizardToken = (FormWizardEntityToken)entityToken;
            var wizard = DynamicFormWizardsFacade.GetWizard(wizardToken.WizardName);

            DynamicFormWizardsFacade.DeleteWizard(wizard);

            new ParentTreeRefresher(flowControllerServicesContainer).PostRefreshMesseges(entityToken);

            return null;
        }
    }
}
