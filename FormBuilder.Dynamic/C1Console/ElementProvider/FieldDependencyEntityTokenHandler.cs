using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.FormBuilder.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider
{
    [Export(typeof(IEntityTokenBasedElementProvider))]
    public class FieldDependencyEntityTokenHandler : IEntityTokenBasedElementProvider
    {
        public bool IsProviderFor(EntityToken token)
        {
            return token is FieldDependencyEntityToken;
        }

        public IEnumerable<Element> Handle(ElementProviderContext context, EntityToken token)
        {
            var dependencyToken = (FieldDependencyEntityToken)token;
            var form = DynamicFormsFacade.GetFormByName(dependencyToken.FormName);

            if (form == null)
            {
                yield break;
            }

            var field = form.Model.Fields.SingleOrDefault(f => f.Name == dependencyToken.FieldName);
            if (field == null)
            {
                yield break;
            }

            if (String.IsNullOrEmpty(dependencyToken.FromFieldName))
            {
                foreach (var dependency in field.DependencyAttributes)
                {
                    var fromFieldName = dependency.ReadFromFieldName;

                    var fieldDependencyElementHandle = context.CreateElementHandle(new FieldDependencyEntityToken(form.Name, field.Name, fromFieldName));
                    var fieldDependencyElement = new Element(fieldDependencyElementHandle)
                    {
                        VisualData = new ElementVisualizedData
                        {
                            Label = fromFieldName,
                            ToolTip = fromFieldName,
                            HasChildren = dependency.ResolveRequiredFieldValues().Any(),
                            Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                            OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
                        }
                    };

                    var editActionToken = new WorkflowActionToken(typeof(EditFieldDependencyWorkflow), new[] { PermissionType.Edit });
                    fieldDependencyElement.AddAction(new ElementAction(new ActionHandle(editActionToken))
                    {
                        VisualData = new ActionVisualizedData
                        {
                            Label = "Edit dependency",
                            ToolTip = "Edit dependency",
                            Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                            ActionLocation = FormBuilderElementProvider.ActionLocation
                        }
                    });

                    var addActionToken = new WorkflowActionToken(typeof(AddFieldDependencyValueWorkflow), new[] { PermissionType.Add });
                    fieldDependencyElement.AddAction(new ElementAction(new ActionHandle(addActionToken))
                    {
                        VisualData = new ActionVisualizedData
                        {
                            Label = "Add value",
                            ToolTip = "Add value",
                            Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                            ActionLocation = FormBuilderElementProvider.ActionLocation
                        }
                    });

                    var deleteActionToken = new ConfirmWorkflowActionToken("Delete: " + fromFieldName, typeof(DeleteFieldDependencyActionToken));
                    fieldDependencyElement.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
                    {
                        VisualData = new ActionVisualizedData
                        {
                            Label = "Delete",
                            ToolTip = "Delete",
                            Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-delete"),
                            ActionLocation = FormBuilderElementProvider.ActionLocation
                        }
                    });

                    yield return fieldDependencyElement;
                }
            }
            else
            {
                var dependency = field.DependencyAttributes.SingleOrDefault(d => d.ReadFromFieldName == dependencyToken.FromFieldName);
                if (dependency == null)
                {
                    yield break;
                }

                foreach (var value in dependency.ResolveRequiredFieldValues())
                {
                    var fieldDependencyValueElementHandle = context.CreateElementHandle(new FieldDependencyValueEntityToken(form.Name, field.Name, dependency.ReadFromFieldName, value.ToString()));
                    var fieldDependencyValueElement = new Element(fieldDependencyValueElementHandle)
                    {
                        VisualData = new ElementVisualizedData
                        {
                            Label = value.ToString(),
                            ToolTip = value.ToString(),
                            HasChildren = false,
                            Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                            OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
                        }
                    };

                    var deleteActionToken = new ConfirmWorkflowActionToken("Delete: " + value, typeof(DeleteFieldDependencyValueActionToken));
                    fieldDependencyValueElement.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
                    {
                        VisualData = new ActionVisualizedData
                        {
                            Label = "Delete",
                            ToolTip = "Delete",
                            Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-delete"),
                            ActionLocation = FormBuilderElementProvider.ActionLocation
                        }
                    });

                    yield return fieldDependencyValueElement;
                }
            }
        }
    }
}
