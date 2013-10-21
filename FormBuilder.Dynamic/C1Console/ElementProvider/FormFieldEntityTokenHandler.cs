using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider
{
    public class FormFieldEntityTokenHandler : IEntityTokenBasedElementProvider
    {
        public Type EntityTokenType
        {
            get { return typeof(FormFieldEntityToken); }
        }

        public IEnumerable<Element> Handle(ElementProviderContext context, EntityToken token)
        {
            var fieldToken = (FormFieldEntityToken)token;
            var form = DynamicFormsFacade.GetFormByName(fieldToken.FormName);

            if (form != null)
            {
                var field = form.Model.Fields.SingleOrDefault(f => f.Name == fieldToken.FieldName);
                if (field != null)
                {
                    var datasourceAttribute = field.Attributes.OfType<DataSourceAttribute>().FirstOrDefault();
                    if (datasourceAttribute != null)
                    {
                        var fieldsElementHandle = context.CreateElementHandle(new FormFieldDataSourceEntityToken(datasourceAttribute.GetType(), form.Name, field.Name));
                        var fieldElement = new Element(fieldsElementHandle)
                        {
                            VisualData = new ElementVisualizedData
                            {
                                Label = "Datasource",
                                ToolTip = "Datasource",
                                HasChildren = true,
                                Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                                OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                            }
                        };

                        var deleteActionToken = new ConfirmWorkflowActionToken("Delete datasource", typeof(DeleteDataSourceActionToken));
                        fieldElement.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
                        {
                            VisualData = new ActionVisualizedData
                            {
                                Label = "Delete datasource",
                                ToolTip = "Delete datasource",
                                Icon = new ResourceHandle("Composite.Icons", "generated-type-data-delete"),
                                ActionLocation = FormBuilderElementProvider.ActionLocation
                            }
                        });

                        if (datasourceAttribute.GetType() == typeof(StringBasedDataSourceAttribute))
                        {
                            var addActionToken = new WorkflowActionToken(typeof(AddStringBasedDataSourceEntryWorkflow), new PermissionType[] { PermissionType.Administrate });
                            fieldElement.AddAction(new ElementAction(new ActionHandle(addActionToken))
                            {
                                VisualData = new ActionVisualizedData
                                {
                                    Label = "Add value",
                                    ToolTip = "Add value",
                                    Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                                    ActionLocation = FormBuilderElementProvider.ActionLocation
                                }
                            });
                        }

                        yield return fieldElement;
                    }
                }
            }
        }
    }
}
