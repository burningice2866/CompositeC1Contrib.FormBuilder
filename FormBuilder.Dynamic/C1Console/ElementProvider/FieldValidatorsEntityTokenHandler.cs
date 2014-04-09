using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider
{
    [Export(typeof(IEntityTokenBasedElementProvider))]
    public class FieldValidatorsEntityTokenHandler : IEntityTokenBasedElementProvider
    {
        public Type EntityTokenType
        {
            get { return typeof(FieldValidatorsEntityToken); }
        }

        public IEnumerable<Element> Handle(ElementProviderContext context, EntityToken token)
        {
            var validatorToken = (FieldValidatorsEntityToken)token;
            var form = DynamicFormsFacade.GetFormByName(validatorToken.FormName);

            if (String.IsNullOrEmpty(validatorToken.Type))
            {
                if (form != null)
                {
                    var field = form.Model.Fields.SingleOrDefault(f => f.Name == validatorToken.FieldName);
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
                                    Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                                    OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                                }
                            };

                            var editActionToken = new WorkflowActionToken(typeof(EditFieldValidatorWorkflow));
                            fieldValidatorElement.AddAction(new ElementAction(new ActionHandle(editActionToken))
                            {
                                VisualData = new ActionVisualizedData
                                {
                                    Label = "Edit",
                                    ToolTip = "Edit",
                                    Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                                    ActionLocation = FormBuilderElementProvider.ActionLocation
                                }
                            });

                            var deleteActionToken = new ConfirmWorkflowActionToken("Delete: " + name, typeof(DeleteFieldValidatorActionToken));
                            fieldValidatorElement.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
                            {
                                VisualData = new ActionVisualizedData
                                {
                                    Label = "Delete",
                                    ToolTip = "Delete",
                                    Icon = new ResourceHandle("Composite.Icons", "generated-type-data-delete"),
                                    ActionLocation = FormBuilderElementProvider.ActionLocation
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
