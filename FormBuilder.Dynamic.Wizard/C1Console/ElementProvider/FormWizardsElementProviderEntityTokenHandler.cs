using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.FormBuilder.C1Console.ElementProvider;
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
            var wizards = DynamicFormWizardsFacade.GetWizards();
            foreach (var wizard in wizards)
            {
                var elementHandle = context.CreateElementHandle(new FormWizardEntityToken(wizard.Name));
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

                yield return wizardElement;
            }
        }
    }
}
