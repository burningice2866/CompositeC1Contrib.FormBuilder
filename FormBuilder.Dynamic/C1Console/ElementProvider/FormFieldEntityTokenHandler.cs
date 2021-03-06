﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider
{
    [Export("FormBuilder", typeof(IElementProviderFor))]
    public class FormFieldEntityTokenHandler : IElementProviderFor
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup(ActionGroupPriority.PrimaryHigh);
        private static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = true, ActionGroup = ActionGroup };

        public IEnumerable<Type> ProviderFor
        {
            get { return new[] { typeof(FormFieldEntityToken) }; }
        }

        public IEnumerable<Element> Provide(ElementProviderContext context, EntityToken token)
        {
            var fieldToken = (FormFieldEntityToken)token;
            var form = DynamicFormsFacade.GetFormByName(fieldToken.FormName);

            if (form == null)
            {
                yield break;
            }

            var field = form.Model.Fields.Get(fieldToken.FieldName);
            if (field == null)
            {
                yield break;
            }

            var fieldValidatorsElementHandle = context.CreateElementHandle(new FieldValidatorsEntityToken(form.Name, field.Name));
            var fieldValidatorsElement = new Element(fieldValidatorsElementHandle)
            {
                VisualData = new ElementVisualizedData
                {
                    Label = "Validators",
                    ToolTip = "Validators",
                    HasChildren = field.ValidationAttributes.Any(),
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                    OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
                }
            };

            var addValidatorActionToken = new WorkflowActionToken(typeof(AddFieldValidatorWorkflow), new[] { PermissionType.Add });
            fieldValidatorsElement.AddAction(new ElementAction(new ActionHandle(addValidatorActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Add validator",
                    ToolTip = "Add validator",
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                    ActionLocation = ActionLocation
                }
            });

            yield return fieldValidatorsElement;

            var fieldDependencyElementHandle = context.CreateElementHandle(new FieldDependencyEntityToken(form.Name, field.Name));
            var fieldDependencyElement = new Element(fieldDependencyElementHandle)
            {
                VisualData = new ElementVisualizedData
                {
                    Label = "Dependencies",
                    ToolTip = "Dependencies",
                    HasChildren = field.DependencyAttributes.Any(),
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                    OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
                }
            };

            var addDependencyActionToken = new WorkflowActionToken(typeof(AddFieldDependencyWorkflow), new[] { PermissionType.Add });
            fieldDependencyElement.AddAction(new ElementAction(new ActionHandle(addDependencyActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Add dependency",
                    ToolTip = "Add dependency",
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                    ActionLocation = ActionLocation
                }
            });

            yield return fieldDependencyElement;

            var datasourceAttribute = field.Attributes.OfType<DataSourceAttribute>().FirstOrDefault();
            if (datasourceAttribute == null)
            {
                yield break;
            }

            var fieldsElementHandle = context.CreateElementHandle(new DataSourceEntityToken(datasourceAttribute.GetType(), form.Name, field.Name));
            var fieldElement = new Element(fieldsElementHandle)
            {
                VisualData = new ElementVisualizedData
                {
                    Label = "Datasource",
                    ToolTip = "Datasource",
                    HasChildren = true,
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                    OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
                }
            };

            var deleteActionToken = new ConfirmWorkflowActionToken("Delete datasource", typeof(DeleteDataSourceActionToken));
            fieldElement.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Delete datasource",
                    ToolTip = "Delete datasource",
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-delete"),
                    ActionLocation = ActionLocation
                }
            });

            if (datasourceAttribute.GetType() == typeof(StringBasedDataSourceAttribute))
            {
                var addActionToken = new WorkflowActionToken(typeof(AddStringBasedDataSourceEntryWorkflow), new[] { PermissionType.Add });
                fieldElement.AddAction(new ElementAction(new ActionHandle(addActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Add value",
                        ToolTip = "Add value",
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                        ActionLocation = ActionLocation
                    }
                });
            }

            yield return fieldElement;
        }
    }
}
