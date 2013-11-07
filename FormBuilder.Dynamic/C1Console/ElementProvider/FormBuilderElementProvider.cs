using System;
using System.Collections.Generic;
using System.Linq;
using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementProvider;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;
using CompositeC1Contrib.FormBuilder.C1Console.Tokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider
{
    public class FormBuilderElementProvider : IHooklessElementProvider
    {
        private static readonly ActionGroup _actionGroup = new ActionGroup(ActionGroupPriority.PrimaryHigh);
        public static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = true, ActionGroup = _actionGroup };

        private static IDictionary<Type, IEntityTokenBasedElementProvider> entityTokenHandlers = new Dictionary<Type, IEntityTokenBasedElementProvider>();

        private ElementProviderContext _context;
        public ElementProviderContext Context
        {
            set { _context = value; }
        }

        static FormBuilderElementProvider()
        {
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in asms)
            {
                try
                {
                    var types = asm.GetTypes()
                        .Where(t => typeof(IEntityTokenBasedElementProvider).IsAssignableFrom(t) && !t.IsInterface)
                        .Select(t => (IEntityTokenBasedElementProvider)Activator.CreateInstance(t));

                    foreach (var t in types)
                    {
                        entityTokenHandlers.Add(t.EntityTokenType, t);
                    }
                }
                catch { }
            }
        }

        public FormBuilderElementProvider() { }

        public IEnumerable<Element> GetChildren(EntityToken entityToken, SearchToken searchToken)
        {
            IEnumerable<Element> elements = Enumerable.Empty<Element>();

            IEntityTokenBasedElementProvider handler;
            if (entityTokenHandlers.TryGetValue(entityToken.GetType(), out handler))
            {
                elements = handler.Handle(_context, entityToken);
            }

            return elements;
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

            var addActionToken = new WorkflowActionToken(typeof(AddFormWorkflow));
            rootElement.AddAction(new ElementAction(new ActionHandle(addActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Add form",
                    ToolTip = "Add form",
                    Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                    ActionLocation = ActionLocation
                }
            });

            return new[] { rootElement };
        }        
    }
}
