using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementProvider;
using Composite.C1Console.Security;
using Composite.C1Console.Trees.Workflows;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;
using Composite.Core.Serialization;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console
{
    public class FormBuilderElementProvider : IHooklessElementProvider
    {
        private static readonly ActionGroup _actionGroup = new ActionGroup(ActionGroupPriority.PrimaryHigh);
        private static readonly ActionLocation _actionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = true, ActionGroup = _actionGroup };

        private ElementProviderContext _context;
        public ElementProviderContext Context
        {
            set { _context = value; }
        }

        public FormBuilderElementProvider() { }

        public IEnumerable<Element> GetChildren(EntityToken entityToken, SearchToken searchToken)
        {
            if (entityToken is FormElementProviderEntityToken)
            {
                var elements = getFormElements();

                foreach (var el in elements)
                {
                    yield return el;
                }
            }

            var formInstanceToken = entityToken as FormInstanceEntityToken;
            if (formInstanceToken != null)
            {
                var folders = getFormFolders(formInstanceToken);

                foreach (var el in folders)
                {
                    yield return el;
                }
            }

            var formFolderToken = entityToken as FormFolderEntityToken;
            if (formFolderToken != null)
            {
                if (formFolderToken.FolderType == FormFolderType.Fields)
                {
                    var elements = getFormFieldElements(formFolderToken);

                    foreach (var el in elements)
                    {
                        yield return el;
                    }
                }
            }

            var fieldToken = entityToken as FormFieldEntityToken;
            if (fieldToken != null)
            {
                var form = DynamicFormsFacade.GetFormByName(fieldToken.Source);
                var field = form.Fields.Single(f => f.Name == fieldToken.Id);
                var datasourceAttribute = field.Attributes.OfType<DataSourceAttribute>().FirstOrDefault();

                if (datasourceAttribute != null)
                {
                    var fieldsElementHandle = _context.CreateElementHandle(new FormFieldDataSourceEntityToken(datasourceAttribute.GetType(), form.Name, field.Name));
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

                    if (datasourceAttribute.GetType() == typeof(StringBasedDataSourceAttribute))
                    {
                        var addActionToken = new WorkflowActionToken(typeof(AddStringBasedDataSourceEntryWorkflow), new PermissionType[] { PermissionType.Administrate });
                        fieldElement.AddAction(new ElementAction(new ActionHandle(addActionToken))
                        {
                            VisualData = new ActionVisualizedData
                            {
                                Label = "Add",
                                ToolTip = "Add",
                                Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                                ActionLocation = _actionLocation
                            }
                        });
                    }

                    yield return fieldElement;
                }
            }

            var dataSourceToken = entityToken as FormFieldDataSourceEntityToken;
            if (dataSourceToken != null)
            {
                var type = Type.GetType(dataSourceToken.Type);
                if (type == typeof(StringBasedDataSourceAttribute))
                {
                    var form = DynamicFormsFacade.GetFormByName(dataSourceToken.FormName);
                    var field = form.Fields.Single(f => f.Name == dataSourceToken.FieldName);
                    var dataSource = field.DataSource;

                    foreach (var entry in dataSource)
                    {
                        var fieldsElementHandle = _context.CreateElementHandle(new StringBasedDataSourceEntryEntityToken(form.Name, field.Name, entry.Key));
                        var fieldElement = new Element(fieldsElementHandle)
                        {
                            VisualData = new ElementVisualizedData
                            {
                                Label = entry.Key,
                                ToolTip = entry.Key,
                                HasChildren = false,
                                Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                                OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                            }
                        };

                        var deleteActionToken = new ConfirmWorkflowActionToken("Delete: " + entry.Key, typeof(DeleteStringBasedDataSourceEntryActionToken));
                        fieldElement.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
                        {
                            VisualData = new ActionVisualizedData
                            {
                                Label = "Delete",
                                ToolTip = "Delete",
                                Icon = new ResourceHandle("Composite.Icons", "generated-type-data-delete"),
                                ActionLocation = _actionLocation
                            }
                        });

                        yield return fieldElement;
                    }
                }
            }
        }

        public IEnumerable<Element> GetRoots(SearchToken searchToken)
        {
            var elementHandle = _context.CreateElementHandle(new FormElementProviderEntityToken());
            var rootElement = new Element(elementHandle)
            {
                VisualData = new ElementVisualizedData
                {
                    Label = "Forms",
                    ToolTip = "Forms",
                    HasChildren = true,
                    Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                    OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                }
            };

            return new[] { rootElement };
        }

        private IEnumerable<Element> getFormFolders(FormInstanceEntityToken token)
        {
            var fieldsElementHandle = _context.CreateElementHandle(new FormFolderEntityToken(token.Id, FormFolderType.Fields));
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

            yield return fieldElement;
        }

        private IEnumerable<Element> getFormElements()
        {
            var forms = DynamicFormsFacade.GetFormDefinitions();
            foreach (var form in forms)
            {
                var label = form.Name;

                var elementHandle = _context.CreateElementHandle(new FormInstanceEntityToken(label));
                var formElement = new Element(elementHandle)
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = label,
                        ToolTip = label,
                        HasChildren = true,
                        Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                        OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                    }
                };

                var deleteActionToken = new ConfirmWorkflowActionToken("Delete: " + label, typeof(DeleteFormActionToken));
                formElement.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Delete",
                        ToolTip = "Delete",
                        Icon = new ResourceHandle("Composite.Icons", "generated-type-data-delete"),
                        ActionLocation = _actionLocation
                    }
                });

                yield return formElement;
            }
        }

        private IEnumerable<Element> getFormFieldElements(FormFolderEntityToken folder)
        {
            var form = DynamicFormsFacade.GetFormByName(folder.Id);

            foreach (var field in form.Fields)
            {
                var elementHandle = _context.CreateElementHandle(new FormFieldEntityToken(form.Name, field.Name));

                var fieldElement = new Element(elementHandle)
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = field.Name,
                        ToolTip = field.Name,
                        HasChildren = true,
                        Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                        OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                    }
                };

                var payload = new StringBuilder();

                StringConversionServices.SerializeKeyValuePair(payload, "_IconResourceName_", "folder");

                var editActionToken = new WorkflowActionToken(typeof(GenericEditDataWorkflow))
                {
                    Payload = payload.ToString()
                };

                fieldElement.AddAction(new ElementAction(new ActionHandle(editActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Edit",
                        ToolTip = "Edit",
                        Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                        ActionLocation = _actionLocation
                    }
                });

                var deleteActionToken = new ConfirmWorkflowActionToken("Delete: " + field.Name, typeof(DeleteFormFieldActionToken));
                fieldElement.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Delete",
                        ToolTip = "Delete",
                        Icon = new ResourceHandle("Composite.Icons", "generated-type-data-delete"),
                        ActionLocation = _actionLocation
                    }
                });

                yield return fieldElement;
            }
        }
    }
}
