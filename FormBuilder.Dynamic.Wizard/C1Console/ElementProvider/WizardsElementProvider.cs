using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Web;

using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementProvider;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.FormBuilder.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.Dynamic.Wizard.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.Wizard.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.Wizard.C1Console.ElementProvider
{
    public class WizardsElementProvider : IHooklessElementProvider
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup(ActionGroupPriority.PrimaryHigh);
        public static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = true, ActionGroup = ActionGroup };

        private static readonly IDictionary<Type, IEntityTokenBasedElementProvider> EntityTokenHandlers = new Dictionary<Type, IEntityTokenBasedElementProvider>();

        private ElementProviderContext _context;
        public ElementProviderContext Context
        {
            set { _context = value; }
        }

        static WizardsElementProvider()
        {
            var batch = new CompositionBatch();
            var catalog = new SafeDirectoryCatalog(HttpRuntime.BinDirectory);
            var container = new CompositionContainer(catalog);

            container.Compose(batch);

            EntityTokenHandlers = container.GetExportedValues<IEntityTokenBasedElementProvider>().ToDictionary(o => o.EntityTokenType, o => o);
        }

        public IEnumerable<Element> GetChildren(EntityToken entityToken, SearchToken searchToken)
        {
            var elements = Enumerable.Empty<Element>();

            IEntityTokenBasedElementProvider handler;
            if (EntityTokenHandlers.TryGetValue(entityToken.GetType(), out handler))
            {
                elements = handler.Handle(_context, entityToken);
            }

            return elements;
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
