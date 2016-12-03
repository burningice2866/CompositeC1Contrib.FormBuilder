using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ActionProviders
{
    [Export("FormBuilder", typeof(IElementActionProviderFor))]
    public class RootElementActionProvider : IElementActionProviderFor
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup(ActionGroupPriority.PrimaryHigh);
        private static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = true, ActionGroup = ActionGroup };

        public IEnumerable<Type> ProviderFor => new[] { typeof(FormElementProviderEntityToken), typeof(NamespaceFolderEntityToken) };

        public void AddActions(Element element)
        {
            var actions = Provide(element.ElementHandle.EntityToken);

            element.AddAction(actions);
        }

        public IEnumerable<ElementAction> Provide(EntityToken entityToken)
        {
            var addFormActionToken = new WorkflowActionToken(typeof(AddFormWorkflow), new[] { PermissionType.Add });
            yield return new ElementAction(new ActionHandle(addFormActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Add form",
                    ToolTip = "Add form",
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                    ActionLocation = ActionLocation
                }
            };

            var addWizardActionToken = new WorkflowActionToken(typeof(AddFormWizardWorkflow), new[] { PermissionType.Add });
            yield return new ElementAction(new ActionHandle(addWizardActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Add wizard",
                    ToolTip = "Add wizard",
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                    ActionLocation = ActionLocation
                }
            };
        }
    }
}
