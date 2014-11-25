using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;
using Composite.Data;
using Composite.Data.Types;

using CompositeC1Contrib.FormBuilder.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.C1Console.Workflows;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider
{
    [Export(typeof(IEntityTokenBasedElementProvider))]
    public class FormElementProviderEntityTokenHandler : IEntityTokenBasedElementProvider
    {
        public Type EntityTokenType
        {
            get { return typeof(FormElementProviderEntityToken); }
        }

        public IEnumerable<Element> Handle(ElementProviderContext context, EntityToken token)
        {
            return GetNamespaceAndFormElements(context, String.Empty);
        }

        public static IEnumerable<Element> GetNamespaceAndFormElements(ElementProviderContext context, string ns)
        {
            var formDefinitions = DefinitionsFacade.GetDefinitions();

            var folders = new List<string>();
            var formElements = new List<Element>();

            if (String.IsNullOrEmpty(ns))
            {
                using (var data = new DataConnection())
                {
                    var homepageIds = data.Get<IPageStructure>().Where(s => s.ParentId == Guid.Empty).Select(s => s.Id);
                    if (homepageIds.Count() > 1)
                    {
                        foreach (var id in homepageIds)
                        {
                            var page = PageManager.GetPageById(id);
                            var sanitizedTitle = SanatizeFormName(page.Title);

                            folders.Add(sanitizedTitle);
                        }
                    }
                }
                
            }
            else
            {
                formDefinitions = formDefinitions.Where(def => def.Name.StartsWith(ns + "."));
            }

            foreach (var definition in formDefinitions)
            {
                var label = definition.Name;

                if (!String.IsNullOrEmpty(ns))
                {
                    label = label.Remove(0, ns.Length + 1);
                }

                var split = label.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length > 1)
                {
                    var folder = split[0];

                    if (!folders.Contains(folder))
                    {
                        folders.Add(folder);
                    }
                }
                else if (split.Length == 1)
                {
                    var formName = label;
                    if (!String.IsNullOrEmpty(ns))
                    {
                        formName = ns + "." + formName;
                    }

                    var elementHandle = context.CreateElementHandle(new FormInstanceEntityToken(typeof(FormBuilderElementProvider).Name, formName));
                    var formElement = new Element(elementHandle)
                    {
                        VisualData = new ElementVisualizedData
                        {
                            Label = label,
                            ToolTip = label,
                            HasChildren = true,
                            Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                            OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
                        }
                    };

                    if (definition is DynamicFormDefinition)
                    {
                        var editActionToken = new WorkflowActionToken(typeof(EditFormWorkflow));
                        formElement.AddAction(new ElementAction(new ActionHandle(editActionToken))
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
                        formElement.AddAction(new ElementAction(new ActionHandle(editRenderingLayoutActionToken))
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

                    if (definition is DynamicFormWizard)
                    {
                        var editActionToken = new WorkflowActionToken(typeof(EditFormWizardWorkflow));
                        formElement.AddAction(new ElementAction(new ActionHandle(editActionToken))
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

                    var deleteActionToken = new ConfirmWorkflowActionToken("Delete: " + label, typeof(DeleteFormActionToken));
                    formElement.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
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
                    formElement.AddAction(new ElementAction(new ActionHandle(copyActionToken))
                    {
                        VisualData = new ActionVisualizedData
                        {
                            Label = "Copy",
                            ToolTip = "Copy",
                            Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                            ActionLocation = FormBuilderElementProvider.ActionLocation
                        }
                    });

                    formElements.Add(formElement);
                }
            }

            foreach (var folder in folders.OrderBy(f => f))
            {
                var handleNamespace = folder;
                if (!String.IsNullOrEmpty(ns))
                {
                    handleNamespace = ns + "." + handleNamespace;
                }

                var folderElement = NamespaceFolderEntityToken.CreateElement(context, typeof(FormBuilderElementProvider).Name, folder, handleNamespace);

                FormBuilderElementProvider.ConfigureFolderActions(folderElement);

                yield return folderElement;
            }

            foreach (var form in formElements)
            {
                yield return form;
            }
        }

        private static string SanatizeFormName(string input)
        {
            var replacementChars = new Dictionary<string, string>
            {
                {"Æ", "Ae" },
                {"æ", "ae"},
                {"Ø", "Oe"},
                {"ø", "oe"},
                {"Å", "Aa"},
                {"å", "aa"},
                {"²", "2"},
                {"&", "og"}
            };

            var illegalChars = new[] { ".", "-", "(", ")", ",", "/", "\"", "§", ":", "?", "+", "Ë", "é" };

            input = illegalChars.Aggregate(input, (current, s) => current.Replace(s, String.Empty));
            input = replacementChars.Aggregate(input, (current, s) => current.Replace(s.Key, s.Value));

            var split = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < split.Length; i++)
            {
                var el = split[i];

                split[i] = el.Substring(0, 1).ToUpper();

                if (el.Length > 1)
                {
                    split[i] += el.Substring(1, el.Length - 1);
    }
}

            return String.Join(String.Empty, split);
        }
    }
}
