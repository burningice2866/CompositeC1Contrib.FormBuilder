using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.FormBuilder.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.Wizard.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Wizard.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Wizard.C1Console.ElementProvider
{
    [Export(typeof(IEntityTokenBasedElementProvider))]
    public class FormWizardEntityTokenHandler : IEntityTokenBasedElementProvider
    {
        public Type EntityTokenType
        {
            get { return typeof(FormWizardEntityToken); }
        }

        public IEnumerable<Element> Handle(ElementProviderContext context, EntityToken token)
        {
            var wizardEntityToken = (FormWizardEntityToken)token;

            var wizard = FormWizardsFacade.GetWizards().SingleOrDefault(w => w.Name == wizardEntityToken.WizardName);
            if (wizard == null)
            {
                yield break;
            }

            var stepsFolderElementHandle = context.CreateElementHandle(new FormWizardFolderEntityToken(wizard.Name, FormWizardFolderType.Steps));
            var stepsFolderElement = new Element(stepsFolderElementHandle)
            {
                VisualData = new ElementVisualizedData
                {
                    Label = "Steps",
                    ToolTip = "Steps",
                    HasChildren = wizard.Steps.Any(),
                    Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                    OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                }
            };

            var addActionToken = new WorkflowActionToken(typeof(AddWizardStepWorkflow));
            stepsFolderElement.AddAction(new ElementAction(new ActionHandle(addActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Add step",
                    ToolTip = "Add step",
                    Icon = new ResourceHandle("Composite.Icons", "generated-type-data-add"),
                    ActionLocation = WizardsElementProvider.ActionLocation
                }
            });

            yield return stepsFolderElement;

            var submitHandlersElementHandle = context.CreateElementHandle(new FormWizardFolderEntityToken(wizard.Name, FormWizardFolderType.SubmitHandlers));
            var submitHandlersElement = new Element(submitHandlersElementHandle)
            {
                VisualData = new ElementVisualizedData
                {
                    Label = "Submit handlers",
                    ToolTip = "Submit handlers",
                    HasChildren = wizard.SubmitHandlers.Any(),
                    Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                    OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                }
            };

            addActionToken = new WorkflowActionToken(typeof(AddSubmitHandlerWorkflow), new[] { PermissionType.Administrate });
            submitHandlersElement.AddAction(new ElementAction(new ActionHandle(addActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Add",
                    ToolTip = "Add",
                    Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                    ActionLocation = WizardsElementProvider.ActionLocation
                }
            });

            yield return submitHandlersElement;
        }
    }
}
