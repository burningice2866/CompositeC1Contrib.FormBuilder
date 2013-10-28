using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Actions;
using Composite.C1Console.Security;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions
{
    [ActionExecutor(typeof(DeleteStringBasedDataSourceActionExecutor))]
    public class DeleteStringBasedDataSourceEntryActionToken : ActionToken
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
            return new DeleteStringBasedDataSourceEntryActionToken();
        }
    }

    public class DeleteStringBasedDataSourceActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var entryToken = (StringBasedDataSourceEntryEntityToken)entityToken;
            var definition = DynamicFormsFacade.GetFormByName(entryToken.FormName);
            var field = definition.Model.Fields.Single(f => f.Name == entryToken.FieldName);
            var datasSource = field.DataSource.ToList();
            var keyToRemove = datasSource.Single(itm => itm.Key == entryToken.Id);

            datasSource.Remove(keyToRemove);

            var dataSourceAttribute = field.Attributes.OfType<StringBasedDataSourceAttribute>().First();

            field.Attributes.Remove(dataSourceAttribute);

            dataSourceAttribute = new StringBasedDataSourceAttribute(datasSource.Select(itm => itm.Key).ToArray());

            field.Attributes.Add(dataSourceAttribute);

            DynamicFormsFacade.SaveForm(definition);

            new ParentTreeRefresher(flowControllerServicesContainer).PostRefreshMesseges(entityToken);

            return null;
        }
    }
}
