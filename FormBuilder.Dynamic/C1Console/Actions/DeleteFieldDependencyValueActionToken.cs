using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Actions;
using Composite.C1Console.Security;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions
{
    [ActionExecutor(typeof(DeleteFieldDependencyValueActionTokenExecutor))]
    public class DeleteFieldDependencyValueActionToken : ActionToken
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
            return new DeleteFieldDependencyValueActionToken();
        }
    }

    public class DeleteFieldDependencyValueActionTokenExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var token = (FieldDependencyValueEntityToken)entityToken;
            var definition = DynamicFormsFacade.GetFormByName(token.FormName);

            var field = definition.Model.Fields.Get(token.FieldName);
            var dependency = (DependsOnConstantAttribute)field.DependencyAttributes.Single(d => d.ReadFromFieldName == token.FromFieldName);
            var valueToRemove = dependency.RequiredFieldValues.Single(v => v.ToString() == token.Value);

            dependency.RequiredFieldValues.Remove(valueToRemove);

            DynamicFormsFacade.SaveForm(definition);

            new ParentTreeRefresher(flowControllerServicesContainer).PostRefreshMessages(entityToken);

            return null;
        }
    }
}
