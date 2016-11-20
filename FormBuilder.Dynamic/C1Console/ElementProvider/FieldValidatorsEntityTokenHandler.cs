using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider.EntityTokenHandlers
{
    [Export("FormBuilder", typeof(IElementProviderFor))]
    public class FieldValidatorsEntityTokenHandler : IElementProviderFor
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup(ActionGroupPriority.PrimaryHigh);
        private static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = true, ActionGroup = ActionGroup };

        public IEnumerable<Type> ProviderFor
        {
            get { return new[] { typeof(FieldValidatorsEntityToken) }; }
        }
        
        public IEnumerable<Element> Provide(ElementProviderContext context, EntityToken token)
        {
            var validatorToken = (FieldValidatorsEntityToken)token;
            var form = DynamicFormsFacade.GetFormByName(validatorToken.FormName);

            if (String.IsNullOrEmpty(validatorToken.Type))
            {
                if (form != null)
                {
                    var field = form.Model.Fields.Get(validatorToken.FieldName);
                    if (field != null)
                    {
                        foreach (var validator in field.ValidationAttributes)
                        {
                            var name = validator.GetType().Name;

                            var fieldValidatorElementHandle = context.CreateElementHandle(new FieldValidatorsEntityToken(form.Name, field.Name, validator.GetType()));
                            var fieldValidatorElement = new Element(fieldValidatorElementHandle)
                            {
                                VisualData = new ElementVisualizedData
                                {
                                    Label = name,
                                    ToolTip = name,
                                    HasChildren = false,
                                    Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                                    OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
                                }
                            };

                            var editActionToken = new WorkflowActionToken(typeof(EditFieldValidatorWorkflow));
                            fieldValidatorElement.AddAction(new ElementAction(new ActionHandle(editActionToken))
                            {
                                VisualData = new ActionVisualizedData
                                {
                                    Label = "Edit",
                                    ToolTip = "Edit",
                                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                                    ActionLocation = ActionLocation
                                }
                            });

                            var deleteActionToken = new ConfirmWorkflowActionToken("Delete: " + name, typeof(DeleteFieldValidatorActionToken));
                            fieldValidatorElement.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
                            {
                                VisualData = new ActionVisualizedData
                                {
                                    Label = "Delete",
                                    ToolTip = "Delete",
                                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-delete"),
                                    ActionLocation = ActionLocation
                                }
                            });

                            yield return fieldValidatorElement;
                        }
                    }
                }
            }
        }
    }
}
