using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider.EntityTokenHandlers
{
    [Export("FormBuilder", typeof(IElementProviderFor))]
    public class DataSourceEntityTokenHandler : IElementProviderFor
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup(ActionGroupPriority.PrimaryHigh);
        private static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = true, ActionGroup = ActionGroup };

        public IEnumerable<Type> ProviderFor
        {
            get { return new[] { typeof(DataSourceEntityToken) }; }
        }

        public IEnumerable<Element> Provide(ElementProviderContext context, EntityToken token)
        {
            var returnList = new List<Element>();

            var dataSourceToken = (DataSourceEntityToken)token;
            var type = Type.GetType(dataSourceToken.Type);

            if (type != typeof(StringBasedDataSourceAttribute))
            {
                return returnList;
            }

            var form = DynamicFormsFacade.GetFormByName(dataSourceToken.FormName);
            if (form == null)
            {
                return returnList;
            }

            var field = form.Model.Fields.Get(dataSourceToken.FieldName);
            if (field == null)
            {
                return returnList;
            }

            var dataSource = field.DataSource;
            if (dataSource == null)
            {
                return returnList;
            }

            foreach (var entry in dataSource)
            {
                var fieldsElementHandle = context.CreateElementHandle(new StringBasedDataSourceEntryEntityToken(form.Name, field.Name, entry.Key));
                var fieldElement = new Element(fieldsElementHandle)
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = entry.Key,
                        ToolTip = entry.Key,
                        HasChildren = false,
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                        OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
                    }
                };

                var deleteActionToken = new ConfirmWorkflowActionToken("Delete: " + entry.Key, typeof(DeleteStringBasedDataSourceEntryActionToken));
                fieldElement.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Delete",
                        ToolTip = "Delete",
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-delete"),
                        ActionLocation = ActionLocation
                    }
                });

                returnList.Add(fieldElement);
            }

            return returnList;
        }
    }
}
