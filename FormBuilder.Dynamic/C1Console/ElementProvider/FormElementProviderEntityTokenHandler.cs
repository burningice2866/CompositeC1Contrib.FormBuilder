using System;
using System.Collections.Generic;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.FormBuilder.C1Console.Tokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider
{
    public class FormElementProviderEntityTokenHandler : IEntityTokenBasedElementProvider
    {
        public Type EntityTokenType
        {
            get { return typeof(FormElementProviderEntityToken); }
        }

        public IEnumerable<Element> Handle(ElementProviderContext context, EntityToken token)
        {
            var elements = getFormElements(context);

            foreach (var el in elements)
            {
                yield return el;
            }
        }

        private IEnumerable<Element> getFormElements(ElementProviderContext context)
        {
            var formDefinitions = DynamicFormsFacade.GetFormDefinitions();
            foreach (var definition in formDefinitions)
            {
                var label = definition.Name;

                var elementHandle = context.CreateElementHandle(new FormInstanceEntityToken(typeof(FormBuilderElementProvider).Name, label));
                var formElement = new Element(elementHandle)
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = label,
                        ToolTip = label,
                        HasChildren = true,
                        Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                        OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                    }
                };

                var editActionToken = new WorkflowActionToken(typeof(EditFormWorkflow));
                formElement.AddAction(new ElementAction(new ActionHandle(editActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Edit",
                        ToolTip = "Edit",
                        Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                        ActionLocation = FormBuilderElementProvider.ActionLocation
                    }
                });

                var deleteActionToken = new ConfirmWorkflowActionToken("Delete: " + label, typeof(DeleteFormActionToken));
                formElement.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Delete",
                        ToolTip = "Delete",
                        Icon = new ResourceHandle("Composite.Icons", "generated-type-data-delete"),
                        ActionLocation = FormBuilderElementProvider.ActionLocation
                    }
                });

                var editRenderingLayoutActionToken = new WorkflowActionToken(typeof(EditFormRenderingLayoutWorkflow));
                formElement.AddAction(new ElementAction(new ActionHandle(editRenderingLayoutActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Edit rendering layout",
                        ToolTip = "Edit rendering layout",
                        Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                        ActionLocation = FormBuilderElementProvider.ActionLocation
                    }
                });

                yield return formElement;
            }
        }
    }
}
