using System.ComponentModel.Composition;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;
using Composite.Data;

using CompositeC1Contrib.FormBuilder.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.C1Console.Workflows;
using CompositeC1Contrib.FormBuilder.Data.Types;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ActionProviders
{
    [Export(typeof(IElementActionProvider))]
    public class FormElementActionProvider : IElementActionProvider
    {
        public bool IsProviderFor(EntityToken entityToken)
        {
            var dataToken = entityToken as DataEntityToken;
            if (dataToken == null)
            {
                return false;
            }

            var form = dataToken.Data as IForm;
            if (form == null)
            {
                return false;
            }

            return true;
        }

        public void AddActions(Element element)
        {
            var form = (IForm)((DataEntityToken)element.ElementHandle.EntityToken).Data;

            var def = DefinitionsFacade.GetDefinition(form.Name);
            if (def == null)
            {
                return;
            }



            if (def is DynamicFormDefinition)
            {
                var editActionToken = new WorkflowActionToken(typeof(EditFormWorkflow));
                element.AddAction(new ElementAction(new ActionHandle(editActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Edit",
                        ToolTip = "Edit",
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                        ActionLocation = FormBuilderElementProvider.ActionLocation
                    }
                });

                var editRenderingLayoutActionToken = new WorkflowActionToken(typeof(EditFormRenderingLayoutWorkflow));
                element.AddAction(new ElementAction(new ActionHandle(editRenderingLayoutActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Edit rendering layout",
                        ToolTip = "Edit rendering layout",
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                        ActionLocation = FormBuilderElementProvider.ActionLocation
                    }
                });
            }

            if (def is DynamicFormWizard)
            {
                var editActionToken = new WorkflowActionToken(typeof(EditFormWizardWorkflow));
                element.AddAction(new ElementAction(new ActionHandle(editActionToken))
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

            var deleteActionToken = new ConfirmWorkflowActionToken("Delete: " + form.Name, typeof(DeleteFormActionToken));
            element.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Delete",
                    ToolTip = "Delete",
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-delete"),
                    ActionLocation = FormBuilderElementProvider.ActionLocation
                }
            });

            var copyActionToken = new WorkflowActionToken(typeof(CopyFormWorkflow));
            element.AddAction(new ElementAction(new ActionHandle(copyActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Copy",
                    ToolTip = "Copy",
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                    ActionLocation = FormBuilderElementProvider.ActionLocation
                }
            });
        }
    }
}
