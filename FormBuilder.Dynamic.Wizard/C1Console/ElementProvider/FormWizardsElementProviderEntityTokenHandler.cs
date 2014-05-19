using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.FormBuilder.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.Wizard.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.Wizard.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.Wizard.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.Wizard.C1Console.ElementProvider
{
    [Export(typeof(IEntityTokenBasedElementProvider))]
    public class FormWizardsElementProviderEntityTokenHandler : IEntityTokenBasedElementProvider
    {
        public Type EntityTokenType
        {
            get { return typeof(FormWizardsElementProviderEntityToken); }
        }

        public IEnumerable<Element> Handle(ElementProviderContext context, EntityToken token)
        {
            return GetNamespaceAndWizardElements(context, String.Empty);
        }

        public static IEnumerable<Element> GetNamespaceAndWizardElements(ElementProviderContext context, string ns)
        {
            var wizards = DynamicFormWizardsFacade.GetWizards();

            var folders = new List<string>();
            var wizardElements = new List<Element>();

            if (!String.IsNullOrEmpty(ns))
            {
                wizards = wizards.Where(def => def.Name.StartsWith(ns));
            }

            foreach (var wizard in wizards)
            {
                var label = wizard.Name;

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
                    var wizardName = label;
                    if (!String.IsNullOrEmpty(ns))
                    {
                        wizardName = ns + "." + wizardName;
                    }

                    var elementHandle = context.CreateElementHandle(new FormWizardEntityToken(wizardName));
                    var wizardElement = new Element(elementHandle)
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

                    var editActionToken = new WorkflowActionToken(typeof(EditFormWizardWorkflow));
                    wizardElement.AddAction(new ElementAction(new ActionHandle(editActionToken))
                    {
                        VisualData = new ActionVisualizedData
                        {
                            Label = "Edit wizard",
                            ToolTip = "Edit wizard",
                            Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                            ActionLocation = WizardsElementProvider.ActionLocation
                        }
                    });

                    var deleteActionToken = new ConfirmWorkflowActionToken("Delete wizard", typeof(DeleteFormWizardActionToken));
                    wizardElement.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
                    {
                        VisualData = new ActionVisualizedData
                        {
                            Label = "Delete wizard",
                            ToolTip = "Delete wizard",
                            Icon = new ResourceHandle("Composite.Icons", "generated-type-data-delete"),
                            ActionLocation = WizardsElementProvider.ActionLocation
                        }
                    });

                    wizardElements.Add(wizardElement);
                }
            }

            foreach (var folder in folders)
            {
                var handleNamespace = folder;
                if (!String.IsNullOrEmpty(ns))
                {
                    handleNamespace = ns + "." + handleNamespace;
                }

                yield return NamespaceFolderEntityToken.CreateElement(context, typeof(WizardsElementProvider).Name, folder, handleNamespace);
            }

            foreach (var wizard in wizardElements)
            {
                yield return wizard;
            }
        }
    }
}
