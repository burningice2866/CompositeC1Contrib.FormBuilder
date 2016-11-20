using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Actions;
using Composite.C1Console.Security;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions
{
    [ActionExecutor(typeof(DeleteFieldDependencyActionTokenExecutor))]
    public class DeleteFieldDependencyActionToken : ActionToken
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
            return new DeleteFieldDependencyActionToken();
        }
    }

    public class DeleteFieldDependencyActionTokenExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var token = (FieldDependencyEntityToken)entityToken;
            var definition = DynamicFormsFacade.GetFormByName(token.FormName);
            var field = definition.Model.Fields.Get(token.FieldName);
            var keyToRemove = field.DependencyAttributes.Single(itm => itm.ReadFromFieldName == token.FromFieldName);

            field.Attributes.Remove(keyToRemove);

            DynamicFormsFacade.SaveForm(definition);

            new ParentTreeRefresher(flowControllerServicesContainer).PostRefreshMesseges(entityToken);

            return null;
        }
    }
}
