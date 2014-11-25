using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider
{
    [Export(typeof(IEntityTokenBasedElementProvider))]
    public class FormFolderEntityTokenHandler : IEntityTokenBasedElementProvider
    {
        public Type EntityTokenType
        {
            get { return typeof(FormFolderEntityToken); }
        }

        public IEnumerable<Element> Handle(ElementProviderContext context, EntityToken token)
        {
            var formFolderToken = (FormFolderEntityToken)token;

            switch (formFolderToken.FolderType)
            {
                case FormFolderType.SubmitHandlers: return getFormSubmitHandlerElements(context, formFolderToken);

                case FormFolderType.Fields: return getFormFieldElements(context, formFolderToken);
                case FormFolderType.Steps: return getWizardStepElements(context, formFolderToken);
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

                var actionProvider = handler as IActionPrivider;
                if (actionProvider != null)
                {
                    actionProvider.AddActions(definition, handlerElement);
                }

                yield return handlerElement;
            }
        }

        private IEnumerable<Element> getFormFieldElements(ElementProviderContext context, FormFolderEntityToken folder)
        {
            var definition = (DynamicFormDefinition)DefinitionsFacade.GetDefinition(folder.FormName);
            if (definition == null)
            {
                yield break;
            }

            foreach (var field in definition.Model.Fields)
            {
                var elementHandle = context.CreateElementHandle(new FormFieldEntityToken(definition.Name, field.Name));
                var fieldElement = new Element(elementHandle)
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = field.Name,
                        ToolTip = field.Name,
                        HasChildren = true,
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                        OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
                    }
                };

                var editActionToken = new WorkflowActionToken(typeof(EditFormFieldWorkflow));
                fieldElement.AddAction(new ElementAction(new ActionHandle(editActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Edit",
                        ToolTip = "Edit",
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                        ActionLocation = FormBuilderElementProvider.ActionLocation
                    }
                });

                var deleteActionToken = new ConfirmWorkflowActionToken("Delete: " + field.Name, typeof(DeleteFormFieldActionToken));
                fieldElement.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Delete",
                        ToolTip = "Delete",
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-delete"),
                        ActionLocation = FormBuilderElementProvider.ActionLocation
                    }
                });

                if (!field.Attributes.OfType<DataSourceAttribute>().Any())
                {
                    var addDataSourceActionToken = new WorkflowActionToken(typeof(AddDataSourceWorkflow));
                    fieldElement.AddAction(new ElementAction(new ActionHandle(addDataSourceActionToken))
                    {
                        VisualData = new ActionVisualizedData
                        {
                            Label = "Add datasource",
                            ToolTip = "Add datasource",
                            Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                            ActionLocation = FormBuilderElementProvider.ActionLocation
                        }
                    });
                }

                yield return fieldElement;
            }
        }

        private IEnumerable<Element> getWizardStepElements(ElementProviderContext context, FormFolderEntityToken folder)
        {
            var wizard = (DynamicFormWizard)DefinitionsFacade.GetDefinition(folder.FormName);
            if (wizard == null)
            {
                yield break;
            }

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
