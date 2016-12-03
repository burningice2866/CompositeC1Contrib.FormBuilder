using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.FormBuilder.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ActionProviders
{
    [Export("FormBuilder", typeof(IElementActionProviderFor))]
    public class SubmitHandlerElementActionProvider : IElementActionProviderFor
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup(ActionGroupPriority.PrimaryHigh);
        private static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = true, ActionGroup = ActionGroup };

        public IEnumerable<Type> ProviderFor => new[] { typeof(FormSubmitHandlerEntityToken) };

        public void AddActions(Element element)
        {
            var actions = Provide(element.ElementHandle.EntityToken);

            element.AddAction(actions);
        }

        public IEnumerable<ElementAction> Provide(EntityToken entityToken)
        {
            var token = (FormSubmitHandlerEntityToken)entityToken;

            var type = Type.GetType(token.Type);
            if (type == null || type != typeof(SaveFormSubmitHandler))
            {
                yield break;
            }

            var downloadActionToken = new DownloadSubmittedFormsActionToken(token.FormName, ".csv");
            yield return new ElementAction(new ActionHandle(downloadActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Download saved forms",
                    ToolTip = "Download saved forms",
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                    ActionLocation = ActionLocation
                }
            };
        }
    }
}
