﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Users;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.C1Console.ActionProviders
{
    [Export("FormBuilder", typeof(IElementActionProviderFor))]
    [ExportMetadata("Order", int.MaxValue)]
    public class FormFieldElementActionProvider : IElementActionProviderFor
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup(ActionGroupPriority.PrimaryHigh);
        private static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Edit, IsInFolder = false, IsInToolbar = true, ActionGroup = ActionGroup };

        public IEnumerable<Type> ProviderFor
        {
            get { return new[] { typeof(FormFieldEntityToken) }; }

        }

        public void AddActions(Element element)
        {
            if (element.Actions.Any(a => a.TagValue == "edit"))
            {
                return;
            }

            var resourceWriter = ResourceFacade.GetResourceWriter(UserSettings.ActiveLocaleCultureInfo);
            if (resourceWriter == null)
            {
                return;
            }

            var token = (FormFieldEntityToken)element.ElementHandle.EntityToken;
            var model = ModelsFacade.GetModel(token.FormName);
            var field = model.Fields.Get(token.FieldName);

            if (!field.Attributes.OfType<FieldLabelAttribute>().Any())
            {
                return;
            }

            var editFieldActionToken = new WorkflowActionToken(typeof(EditFormFieldSettingsWorkflow));

            element.AddAction(new ElementAction(new ActionHandle(editFieldActionToken))
            {
                TagValue = "edit",

                VisualData = new ActionVisualizedData
                {
                    Label = "Edit",
                    ToolTip = "Edit",
                    Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                    ActionLocation = ActionLocation
                }
            });
        }

        public IEnumerable<ElementAction> Provide(EntityToken entityToken)
        {
            return Enumerable.Empty<ElementAction>();
        }
    }
}
