using System.ComponentModel.Composition;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ActionProviders
{
    [Export(typeof(IElementActionProvider))]
    public class FormFieldElementActionProvider : IElementActionProvider
    {
        public bool IsProviderFor(EntityToken entityToken)
        {
            return entityToken is FormFieldEntityToken;
        }

        public void AddActions(Element element)
        {
            var token = (FormFieldEntityToken)element.ElementHandle.EntityToken;
            var defintion = DynamicFormsFacade.GetFormByName(token.FormName);
            if (defintion == null)
            {
                return;
            }

            var field = defintion.Model.Fields.SingleOrDefault(f => f.Name == token.FieldName);
            if (field == null)
            {
                return;
            }

            var editActionToken = new WorkflowActionToken(typeof(EditFormFieldWorkflow));
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

            var deleteActionToken = new ConfirmWorkflowActionToken("Delete: " + field.Name, typeof(DeleteFormFieldActionToken));
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

            if (!field.Attributes.OfType<DataSourceAttribute>().Any())
            {
                var addDataSourceActionToken = new WorkflowActionToken(typeof(AddDataSourceWorkflow));
                element.AddAction(new ElementAction(new ActionHandle(addDataSourceActionToken))
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
        }
    }
}
