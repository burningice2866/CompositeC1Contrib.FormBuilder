using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.FormBuilder.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider
{
    [Export(typeof(IEntityTokenBasedElementProvider))]
    public class FormFolderEntityTokenHandler : IEntityTokenBasedElementProvider
    {
        public bool IsProviderFor(EntityToken token)
        {
            return token is FormFolderEntityToken;
        }

        public IEnumerable<Element> Handle(ElementProviderContext context, EntityToken token)
        {
            var formFolderToken = (FormFolderEntityToken)token;

            switch (formFolderToken.FolderType)
            {
                case "SubmitHandlers": return getFormSubmitHandlerElements(context, formFolderToken);
                case "Steps": return getWizardStepElements(context, formFolderToken);
            }

            return Enumerable.Empty<Element>();
        }

        private IEnumerable<Element> getFormSubmitHandlerElements(ElementProviderContext context, FormFolderEntityToken folder)
        {
            var definition = DefinitionsFacade.GetDefinition(folder.FormName);
            if (definition == null)
            {
                yield break;
            }

            foreach (var handler in definition.SubmitHandlers)
            {
                var elementHandle = context.CreateElementHandle(new FormSubmitHandlerEntityToken(handler.GetType(), folder.FormName, handler.Name));
                var handlerElement = new Element(elementHandle)
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = handler.Name,
                        ToolTip = handler.Name,
                        HasChildren = false,
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                        OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
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
                            Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                            ActionLocation = FormBuilderElementProvider.ActionLocation
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
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-delete"),
                        ActionLocation = FormBuilderElementProvider.ActionLocation
                    }
                });

                yield return handlerElement;
            }
        }

        private IEnumerable<Element> getWizardStepElements(ElementProviderContext context, FormFolderEntityToken folder)
        {
            var wizard = (DynamicWizardDefinition)DefinitionsFacade.GetDefinition(folder.FormName);
            if (wizard == null)
            {
                yield break;
            }

            foreach (var step in wizard.Model.Steps)
            {
                var elementHandle = context.CreateElementHandle(new FormWizardStepEntityToken(wizard.Name, step.Name));
                var wizardStepElement = new Element(elementHandle)
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = step.Name,
                        ToolTip = step.Name,
                        HasChildren = false,
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                        OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
                    }
                };

                var editActionToken = new WorkflowActionToken(typeof(EditWizardStepWorkflow));
                wizardStepElement.AddAction(new ElementAction(new ActionHandle(editActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Edit step",
                        ToolTip = "Edit step",
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                        ActionLocation = FormBuilderElementProvider.ActionLocation
                    }
                });

                var deleteActionToken = new ConfirmWorkflowActionToken("Delete step", typeof(DeleteFormWizardStepActionToken));
                wizardStepElement.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Delete step",
                        ToolTip = "Delete step",
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-delete"),
                        ActionLocation = FormBuilderElementProvider.ActionLocation
                    }
                });

                yield return wizardStepElement;
            }
        }
    }
}
