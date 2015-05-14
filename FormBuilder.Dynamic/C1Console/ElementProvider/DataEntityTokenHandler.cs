using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;
using Composite.Core.WebClient;
using Composite.Data;

using CompositeC1Contrib.FormBuilder.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.Data.Types;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider
{
    [Export(typeof(IEntityTokenBasedElementProvider))]
    public class DataEntityTokenHandler : IEntityTokenBasedElementProvider
    {
        public Type EntityTokenType
        {
            get { return typeof(DataEntityToken); }
        }

        public IEnumerable<Element> Handle(ElementProviderContext context, EntityToken token)
        {
            var folders = getFormFolders(context, (DataEntityToken)token);

            return folders;
        }

        private IEnumerable<Element> getFormFolders(ElementProviderContext context, DataEntityToken token)
        {
            var form = (IForm)(token).Data;
            if (form == null)
            {
                yield break;
            }

            var def = DefinitionsFacade.GetDefinition(form.Name);

            if (def is DynamicFormDefinition)
            {
                var fieldsElementHandle = context.CreateElementHandle(new FormFolderEntityToken(form.Name, FormFolderType.Fields));
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

                var addActionToken = new WorkflowActionToken(typeof(AddFormFieldWorkflow), new[] { PermissionType.Add });
                fieldElement.AddAction(new ElementAction(new ActionHandle(addActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Add field",
                        ToolTip = "Add field",
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                        ActionLocation = FormBuilderElementProvider.ActionLocation
                    }
                });

                var url = String.Format("InstalledPackages/CompositeC1Contrib.FormBuilder.Dynamic/SortFormFields.aspx?formName={0}", form.Name);
                var sortActionToken = new UrlActionToken("Sort fields", UrlUtils.ResolveAdminUrl(url), new[] { PermissionType.Edit });
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
                var stepsFolderElementHandle = context.CreateElementHandle(new FormFolderEntityToken(form.Name, FormFolderType.Steps));
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

                var addActionToken = new WorkflowActionToken(typeof(AddWizardStepWorkflow), new[] { PermissionType.Add });
                stepsFolderElement.AddAction(new ElementAction(new ActionHandle(addActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Add step",
                        ToolTip = "Add step",
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                        ActionLocation = FormBuilderElementProvider.ActionLocation
                    }
                });

                var url = String.Format("InstalledPackages/CompositeC1Contrib.FormBuilder.Dynamic/SortWizardSteps.aspx?wizardName={0}", form.Name);
                var sortActionToken = new UrlActionToken("Sort fields", UrlUtils.ResolveAdminUrl(url), new[] { PermissionType.Edit });
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

            var submitHandlersElementHandle = context.CreateElementHandle(new FormFolderEntityToken(form.Name, FormFolderType.SubmitHandlers));
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

            var submitHandlerActionToken = new WorkflowActionToken(typeof(AddSubmitHandlerWorkflow), new[] { PermissionType.Add });
            submitHandlersElement.AddAction(new ElementAction(new ActionHandle(submitHandlerActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Add submit handler",
                    ToolTip = "Add submit handler",
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                    ActionLocation = FormBuilderElementProvider.ActionLocation
                }
            });

            yield return submitHandlersElement;
        }
    }
}
