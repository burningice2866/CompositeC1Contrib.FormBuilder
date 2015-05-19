using System.ComponentModel.Composition;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.FormBuilder.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ActionProviders
{
    [Export(typeof(IElementActionProvider))]
    public class RootElementActionProvider : IElementActionProvider
    {
        public bool IsProviderFor(EntityToken entityToken)
        {
            return entityToken is FormElementProviderEntityToken || entityToken is NamespaceFolderEntityToken;
        }

        public void AddActions(Element element)
        {
            var addFormActionToken = new WorkflowActionToken(typeof(AddFormWorkflow), FormBuilderElementProvider.AddPermissions);
            element.AddAction(new ElementAction(new ActionHandle(addFormActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Add form",
                    ToolTip = "Add form",
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                    ActionLocation = FormBuilderElementProvider.ActionLocation
                }
            });

            var addWizardActionToken = new WorkflowActionToken(typeof(AddFormWizardWorkflow), FormBuilderElementProvider.AddPermissions);
            element.AddAction(new ElementAction(new ActionHandle(addWizardActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Add wizard",
                    ToolTip = "Add wizard",
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                    ActionLocation = FormBuilderElementProvider.ActionLocation
                }
            });
        }
    }
}
