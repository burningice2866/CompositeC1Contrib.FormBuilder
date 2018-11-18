using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Actions;
using Composite.C1Console.Security;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions
{
    [ActionExecutor(typeof(DeleteStringBasedDataSourceActionExecutor))]
    public class DeleteStringBasedDataSourceEntryActionToken : ActionToken
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
            return new DeleteStringBasedDataSourceEntryActionToken();
        }
    }

    public class DeleteStringBasedDataSourceActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var entryToken = (StringBasedDataSourceEntryEntityToken)entityToken;
            var definition = DynamicFormsFacade.GetFormByName(entryToken.FormName);
            var field = definition.Model.Fields.Get(entryToken.FieldName);
            var dataSource = field.DataSource.ToList();

            dataSource.Remove(dataSource.Get(entryToken.Id));

            var dataSourceAttribute = field.Attributes.OfType<StringBasedDataSourceAttribute>().First();

            field.Attributes.Remove(dataSourceAttribute);

            dataSourceAttribute = new StringBasedDataSourceAttribute(dataSource.Select(itm => itm.Key).ToArray());

            field.Attributes.Add(dataSourceAttribute);

            DynamicFormsFacade.SaveForm(definition);

            new ParentTreeRefresher(flowControllerServicesContainer).PostRefreshMessages(entityToken);

            return null;
        }
    }
}
