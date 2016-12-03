using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ActionProviders
{
    [Export("FormBuilder", typeof(IElementActionProviderFor))]
    public class FormFieldElementActionProvider : IElementActionProviderFor
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup(ActionGroupPriority.PrimaryHigh);
        private static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = true, ActionGroup = ActionGroup };

        public IEnumerable<Type> ProviderFor => new[] { typeof(FormFieldEntityToken) };

        public void AddActions(Element element)
        {
            var actions = Provide(element.ElementHandle.EntityToken);

            element.AddAction(actions);
        }

        public IEnumerable<ElementAction> Provide(EntityToken entityToken)
        {
            var token = (FormFieldEntityToken)entityToken;

            var defintion = DynamicFormsFacade.GetFormByName(token.FormName);
            if (defintion == null)
            {
                yield break;
            }

            var field = defintion.Model.Fields.Get(token.FieldName);
            if (field == null)
            {
                yield break;
            }

            var editActionToken = new WorkflowActionToken(typeof(EditFormFieldWorkflow));
            yield return new ElementAction(new ActionHandle(editActionToken))
            {
                TagValue = "edit",

                VisualData = new ActionVisualizedData
                {
                    Label = "Edit",
                    ToolTip = "Edit",
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                    ActionLocation = ActionLocation
                }
            };

            var deleteActionToken = new ConfirmWorkflowActionToken("Delete: " + field.Name, typeof(DeleteFormFieldActionToken));
            yield return new ElementAction(new ActionHandle(deleteActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Delete",
                    ToolTip = "Delete",
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-delete"),
                    ActionLocation = ActionLocation
                }
            };

            if (!field.Attributes.OfType<DataSourceAttribute>().Any())
            {
                var addDataSourceActionToken = new WorkflowActionToken(typeof(AddDataSourceWorkflow));
                yield return new ElementAction(new ActionHandle(addDataSourceActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Add datasource",
                        ToolTip = "Add datasource",
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                        ActionLocation = ActionLocation
                    }
                };
            }
        }
    }
}
