using System;
using System.Collections.Generic;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

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

            yield return fieldElement;
        }
    }
}
