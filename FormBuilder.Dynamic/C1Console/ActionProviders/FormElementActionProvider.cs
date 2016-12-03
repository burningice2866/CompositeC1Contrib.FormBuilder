using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;
using Composite.Data;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.FormBuilder.C1Console.Workflows;
using CompositeC1Contrib.FormBuilder.Data.Types;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ActionProviders
{
    [Export("FormBuilder", typeof(IElementActionProviderFor))]
    public class FormElementActionProvider : IElementActionProviderFor
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup(ActionGroupPriority.PrimaryHigh);
        private static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = true, ActionGroup = ActionGroup };

        public IEnumerable<Type> ProviderFor => new[] { typeof(DataEntityToken) };

        public void AddActions(Element element)
        {
            var actions = Provide(element.ElementHandle.EntityToken);

            element.AddAction(actions);
        }

        public IEnumerable<ElementAction> Provide(EntityToken entityToken)
        {
            var dataEntityToken = (DataEntityToken)entityToken;

            var modelReference = dataEntityToken.Data as IModelReference;
            if (modelReference == null)
            {
                yield break;
            }

            var def = DefinitionsFacade.GetDefinition(modelReference.Name);
            if (def == null)
            {
                yield break;
            }

            if (def is DynamicFormDefinition)
            {
                var editActionToken = new WorkflowActionToken(typeof(EditFormWorkflow));
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

                var editRenderingLayoutActionToken = new WorkflowActionToken(typeof(EditFormRenderingLayoutWorkflow));
                yield return new ElementAction(new ActionHandle(editRenderingLayoutActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Edit rendering layout",
                        ToolTip = "Edit rendering layout",
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                        ActionLocation = ActionLocation
                    }
                };
            }

            if (def is DynamicWizardDefinition)
            {
                var editActionToken = new WorkflowActionToken(typeof(EditFormWizardWorkflow));
                yield return new ElementAction(new ActionHandle(editActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Edit",
                        ToolTip = "Edit",
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                        ActionLocation = ActionLocation
                    }
                };
            }

            var deleteActionToken = new ConfirmWorkflowActionToken("Delete: " + modelReference.Name, typeof(DeleteFormActionToken));
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

            var copyActionToken = new WorkflowActionToken(typeof(CopyFormWorkflow));
            yield return new ElementAction(new ActionHandle(copyActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Copy",
                    ToolTip = "Copy",
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                    ActionLocation = ActionLocation
                }
            };
        }
    }
}
