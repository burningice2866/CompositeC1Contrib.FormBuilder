using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.FormBuilder.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.Wizard.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Wizard.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Wizard.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Wizard.C1Console.ElementProvider
{
    [Export(typeof(IEntityTokenBasedElementProvider))]
    public class FormWizardFolderEntityTokenHandler : IEntityTokenBasedElementProvider
    {
        public Type EntityTokenType
        {
            get { return typeof(FormWizardFolderEntityToken); }
        }

        public IEnumerable<Element> Handle(ElementProviderContext context, EntityToken token)
        {
            var wizardFolderEntityToken = (FormWizardFolderEntityToken)token;

            var wizard = FormWizardsFacade.GetWizards().SingleOrDefault(w => w.Name == wizardFolderEntityToken.WizardName);
            if (wizard != null)
            {
                switch (wizardFolderEntityToken.FolderType)
                {
                    case FormWizardFolderType.Steps: return HandleStepsFolder(context, wizard);
                    case FormWizardFolderType.SubmitHandlers: return HandleSubmitHandlersFolder(context, wizard);
                }
            }

            return Enumerable.Empty<Element>();
        }

        private static IEnumerable<Element> HandleSubmitHandlersFolder(ElementProviderContext context, FormWizard wizard)
        {
            foreach (var handler in wizard.SubmitHandlers)
            {
                var elementHandle = context.CreateElementHandle(new FormSubmitHandlerEntityToken(handler.GetType(), wizard.Name, handler.Name));
                var handlerElement = new Element(elementHandle)
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = handler.Name,
                        ToolTip = handler.Name,
                        HasChildren = false,
                        Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                        OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                    }
                };

                var editWorkflowAttribute = handler.GetType().GetCustomAttributes(true).OfType<EditWorkflowAttribute>().FirstOrDefault();
                if (editWorkflowAttribute != null)
                {
                    var editActionToken = new WorkflowActionToken(editWorkflowAttribute.EditWorkflowType);
                    handlerElement.AddAction(new ElementAction(new ActionHandle(editActionToken))
                    {
                        VisualData = new ActionVisualizedData
                        {
                            Label = "Edit",
                            ToolTip = "Edit",
                            Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                            ActionLocation = WizardsElementProvider.ActionLocation
                        }
                    });
                }

                var deleteActionToken = new ConfirmWorkflowActionToken("Delete: " + handler.Name, typeof(DeleteSubmitHandlerActionToken));
                handlerElement.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Delete",
                        ToolTip = "Delete",
                        Icon = new ResourceHandle("Composite.Icons", "generated-type-data-delete"),
                        ActionLocation = WizardsElementProvider.ActionLocation
                    }
                });

                yield return handlerElement;
            }
        }

        private static IEnumerable<Element> HandleStepsFolder(ElementProviderContext context, FormWizard wizard)
        {
            foreach (var step in wizard.Steps)
            {
                var elementHandle = context.CreateElementHandle(new FormWizardStepEntityToken(wizard.Name, step.Name));
                var wizardStepElement = new Element(elementHandle)
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = step.Name,
                        ToolTip = step.Name,
                        HasChildren = false,
                        Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                        OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                    }
                };

                var editActionToken = new WorkflowActionToken(typeof(EditWizardStepWorkflow));
                wizardStepElement.AddAction(new ElementAction(new ActionHandle(editActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Edit step",
                        ToolTip = "Edit step",
                        Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                        ActionLocation = WizardsElementProvider.ActionLocation
                    }
                });

                var deleteActionToken = new ConfirmWorkflowActionToken("Delete step", typeof(DeleteFormWizardStepActionToken));
                wizardStepElement.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Delete step",
                        ToolTip = "Delete step",
                        Icon = new ResourceHandle("Composite.Icons", "generated-type-data-delete"),
                        ActionLocation = WizardsElementProvider.ActionLocation
                    }
                });

                yield return wizardStepElement;
            }
        }
    }
}
