using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementProvider;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.FormBuilder.Wizard.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Wizard.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Wizard.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Wizard.C1Console.ElementProvider
{
    public class WizardsElementProvider : IHooklessElementProvider
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup(ActionGroupPriority.PrimaryHigh);
        public static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = true, ActionGroup = ActionGroup };

        private ElementProviderContext _context;
        public ElementProviderContext Context
        {
            set { _context = value; }
        }

        public IEnumerable<Element> GetChildren(EntityToken entityToken, SearchToken searchToken)
        {
            if (entityToken is FormWizardsElementProviderEntityToken)
            {
                var wizards = FormWizardsFacade.GetWizards();
                foreach (var wizard in wizards)
                {
                    var elementHandle = _context.CreateElementHandle(new FormWizardEntityToken(wizard.Name));
                    var wizardElement = new Element(elementHandle)
                    {
                        VisualData = new ElementVisualizedData
                        {
                            Label = wizard.Name,
                            ToolTip = wizard.Name,
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
                            ActionLocation = ActionLocation
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
                            ActionLocation = ActionLocation
                        }
                    });

                    var addActionToken = new WorkflowActionToken(typeof(AddWizardStepWorkflow));
                    wizardElement.AddAction(new ElementAction(new ActionHandle(addActionToken))
                    {
                        VisualData = new ActionVisualizedData
                        {
                            Label = "Add step",
                            ToolTip = "Add step",
                            Icon = new ResourceHandle("Composite.Icons", "generated-type-data-add"),
                            ActionLocation = ActionLocation
                        }
                    });

                    yield return wizardElement;
                }
            }

            var wizardEntityToken = entityToken as FormWizardEntityToken;
            if (wizardEntityToken != null)
            {
                var wizard = FormWizardsFacade.GetWizards().SingleOrDefault(w => w.Name == wizardEntityToken.WizardName);
                if (wizard != null)
                {
                    foreach (var step in wizard.Steps)
                    {
                        var elementHandle =
                            _context.CreateElementHandle(new FormWizardStepEntityToken(wizard.Name, step.Name));
                        var wizardStepElement = new Element(elementHandle)
                        {
                            VisualData = new ElementVisualizedData
                            {
                                Label = step.Name,
                                ToolTip = step.Name,
                                HasChildren = false,
                                Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                                OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                            }
                        };

                        var editActionToken = new WorkflowActionToken(typeof(EditWizardStepWorkflow));
                        wizardStepElement.AddAction(new ElementAction(new ActionHandle(editActionToken))
                        {
                            VisualData = new ActionVisualizedData
                            {
                                Label = "Edit step",
                                ToolTip = "Edit step",
                                Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                                ActionLocation = ActionLocation
                            }
                        });

                        var deleteActionToken = new ConfirmWorkflowActionToken("Delete step", typeof(DeleteFormWizardStepActionToken));
                        wizardStepElement.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
                        {
                            VisualData = new ActionVisualizedData
                            {
                                Label = "Delete step",
                                ToolTip = "Delete step",
                                Icon = new ResourceHandle("Composite.Icons", "generated-type-data-delete"),
                                ActionLocation = ActionLocation
                            }
                        });

                        yield return wizardStepElement;
                    }
                }
            }
        }

        public IEnumerable<Element> GetRoots(SearchToken searchToken)
        {
            var elementHandle = _context.CreateElementHandle(new FormWizardsElementProviderEntityToken());
            var rootElement = new Element(elementHandle)
            {
                VisualData = new ElementVisualizedData
                {
                    Label = "Form wizards",
                    ToolTip = "Form wizards",
                    HasChildren = true,
                    Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                    OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                }
            };

            var addActionToken = new WorkflowActionToken(typeof(AddFormWizardWorkflow));
            rootElement.AddAction(new ElementAction(new ActionHandle(addActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Add wizard",
                    ToolTip = "Add wizard",
                    Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                    ActionLocation = ActionLocation
                }
            });

            return new[] { rootElement };
        }
    }
}
