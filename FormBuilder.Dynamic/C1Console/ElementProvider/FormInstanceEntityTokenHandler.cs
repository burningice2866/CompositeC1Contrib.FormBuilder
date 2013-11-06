using System;
using System.Collections.Generic;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;
using Composite.Core.WebClient;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider
{
    public class FormInstanceEntityTokenHandler : IEntityTokenBasedElementProvider
    {
        public Type EntityTokenType
        {
            get { return typeof(FormInstanceEntityToken); }
        }

        public IEnumerable<Element> Handle(ElementProviderContext context, EntityToken token)
        {
            var folders = getFormFolders(context, (FormInstanceEntityToken)token);

            foreach (var el in folders)
            {
                yield return el;
            }
        }

        private IEnumerable<Element> getFormFolders(ElementProviderContext context, FormInstanceEntityToken token)
        {
            var fieldsElementHandle = context.CreateElementHandle(new FormFolderEntityToken(token.FormName, FormFolderType.Fields));
            var fieldElement = new Element(fieldsElementHandle)
            {
                VisualData = new ElementVisualizedData
                {
                    Label = "Fields",
                    ToolTip = "Fields",
                    HasChildren = true,
                    Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                    OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                }
            };

            var addActionToken = new WorkflowActionToken(typeof(AddFormFieldWorkflow), new PermissionType[] { PermissionType.Administrate });
            fieldElement.AddAction(new ElementAction(new ActionHandle(addActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Add",
                    ToolTip = "Add",
                    Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                    ActionLocation = FormBuilderElementProvider.ActionLocation
                }
            });

            var url = String.Format("InstalledPackages/CompositeC1Contrib.FormBuilder.Dynamic/SortFormFields.aspx?formName={0}", token.FormName);
            var sortActionToken = new UrlActionToken("Sort fields", UrlUtils.ResolveAdminUrl(url), new PermissionType[] { PermissionType.Administrate });
            fieldElement.AddAction(new ElementAction(new ActionHandle(sortActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Sort fields",
                    ToolTip = "Sort fields",
                    Icon = new ResourceHandle("Composite.Icons", "cut"),
                    ActionLocation = FormBuilderElementProvider.ActionLocation
                }
            });

            yield return fieldElement;

            var submitHandlersElementHandle = context.CreateElementHandle(new FormFolderEntityToken(token.FormName, FormFolderType.SubmitHandlers));
            var submitHandlersElement = new Element(submitHandlersElementHandle)
            {
                VisualData = new ElementVisualizedData
                {
                    Label = "Submit handlers",
                    ToolTip = "Submit handlers",
                    HasChildren = true,
                    Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                    OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                }
            };

            addActionToken = new WorkflowActionToken(typeof(AddSubmitHandlerWorkflow), new PermissionType[] { PermissionType.Administrate });
            submitHandlersElement.AddAction(new ElementAction(new ActionHandle(addActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Add",
                    ToolTip = "Add",
                    Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                    ActionLocation = FormBuilderElementProvider.ActionLocation
                }
            });

            yield return submitHandlersElement;
        }
    }
}
