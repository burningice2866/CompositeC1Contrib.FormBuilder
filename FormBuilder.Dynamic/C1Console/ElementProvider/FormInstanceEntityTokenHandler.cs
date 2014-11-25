using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;
using Composite.Core.WebClient;

using CompositeC1Contrib.FormBuilder.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider
{
    [Export(typeof(IEntityTokenBasedElementProvider))]
    public class FormInstanceEntityTokenHandler : IEntityTokenBasedElementProvider
    {
        public Type EntityTokenType
        {
            get { return typeof(FormInstanceEntityToken); }
        }

        public IEnumerable<Element> Handle(ElementProviderContext context, EntityToken token)
        {
            var folders = getFormFolders(context, (FormInstanceEntityToken)token);

            return folders;
        }

        private IEnumerable<Element> getFormFolders(ElementProviderContext context, FormInstanceEntityToken token)
        {
            var def = DefinitionsFacade.GetDefinition(token.FormName);

            if (def is DynamicFormDefinition)
            {
                var fieldsElementHandle = context.CreateElementHandle(new FormFolderEntityToken(token.FormName, FormFolderType.Fields));
                var fieldElement = new Element(fieldsElementHandle)
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = "Fields",
                        ToolTip = "Fields",
                        HasChildren = true,
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                        OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
                    }
                };

                var addActionToken = new WorkflowActionToken(typeof(AddFormFieldWorkflow), new[] { PermissionType.Administrate });
                fieldElement.AddAction(new ElementAction(new ActionHandle(addActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Add",
                        ToolTip = "Add",
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                        ActionLocation = FormBuilderElementProvider.ActionLocation
                    }
                });

                var url = String.Format("InstalledPackages/CompositeC1Contrib.FormBuilder.Dynamic/SortFormFields.aspx?formName={0}", token.FormName);
                var sortActionToken = new UrlActionToken("Sort fields", UrlUtils.ResolveAdminUrl(url), new[] { PermissionType.Administrate });
                fieldElement.AddAction(new ElementAction(new ActionHandle(sortActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Sort fields",
                        ToolTip = "Sort fields",
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("cut"),
                        ActionLocation = FormBuilderElementProvider.ActionLocation
                    }
                });

                yield return fieldElement;
            }

            if (def is DynamicFormWizard)
            {
                var stepsFolderElementHandle = context.CreateElementHandle(new FormFolderEntityToken(token.FormName, FormFolderType.Steps));
                var stepsFolderElement = new Element(stepsFolderElementHandle)
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = "Steps",
                        ToolTip = "Steps",
                        HasChildren = true,
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                        OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
                    }
                };

                var addActionToken = new WorkflowActionToken(typeof(AddWizardStepWorkflow), new[] { PermissionType.Administrate });
                stepsFolderElement.AddAction(new ElementAction(new ActionHandle(addActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Add",
                        ToolTip = "Add",
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                        ActionLocation = FormBuilderElementProvider.ActionLocation
                    }
                });

                var url = String.Format("InstalledPackages/CompositeC1Contrib.FormBuilder.Dynamic/SortWizardSteps.aspx?wizardName={0}", token.FormName);
                var sortActionToken = new UrlActionToken("Sort fields", UrlUtils.ResolveAdminUrl(url), new[] { PermissionType.Administrate });
                stepsFolderElement.AddAction(new ElementAction(new ActionHandle(sortActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Sort steps",
                        ToolTip = "Sort steps",
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("cut"),
                        ActionLocation = FormBuilderElementProvider.ActionLocation
                    }
                });

                yield return stepsFolderElement;
            }

            var submitHandlersElementHandle = context.CreateElementHandle(new FormFolderEntityToken(token.FormName, FormFolderType.SubmitHandlers));
            var submitHandlersElement = new Element(submitHandlersElementHandle)
            {
                VisualData = new ElementVisualizedData
                {
                    Label = "Submit handlers",
                    ToolTip = "Submit handlers",
                    HasChildren = true,
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                    OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
                }
            };

            var submitHandlerActionToken = new WorkflowActionToken(typeof(AddSubmitHandlerWorkflow), new[] { PermissionType.Administrate });
            submitHandlersElement.AddAction(new ElementAction(new ActionHandle(submitHandlerActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Add",
                    ToolTip = "Add",
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                    ActionLocation = FormBuilderElementProvider.ActionLocation
                }
            });

            yield return submitHandlersElement;
        }
    }
}
