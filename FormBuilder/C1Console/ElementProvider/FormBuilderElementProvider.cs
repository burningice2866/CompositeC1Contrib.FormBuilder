using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementProvider;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;
using Composite.Data;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Data.Types;

namespace CompositeC1Contrib.FormBuilder.C1Console.ElementProvider
{
    public class FormBuilderElementProvider : IHooklessElementProvider, IAuxiliarySecurityAncestorProvider
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup(ActionGroupPriority.PrimaryHigh);

        private static readonly IList<IEntityTokenBasedElementProvider> EntityTokenHandlers;
        private static readonly IList<IElementActionProvider> ElementActionProviders;

        public static readonly IEnumerable<PermissionType> AddPermissions = new[] { PermissionType.Add };
        public static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = true, ActionGroup = ActionGroup };

        private ElementProviderContext _context;
        public ElementProviderContext Context
        {
            set { _context = value; }
        }

        static FormBuilderElementProvider()
        {
            var elementProviders = CompositionContainerFacade.GetExportedValues<IEntityTokenBasedElementProvider>().ToList();
            var actionProviders = CompositionContainerFacade.GetExportedValues<IElementActionProvider>().ToList();

            EntityTokenHandlers = elementProviders;
            ElementActionProviders = actionProviders;
        }

        public IEnumerable<Element> GetChildren(EntityToken entityToken, SearchToken searchToken)
        {
            var elements = new List<Element>();

            foreach (var handler in EntityTokenHandlers.Where(h => h.IsProviderFor(entityToken)))
            {
                elements.AddRange(handler.Handle(_context, entityToken));
            }

            foreach (var el in elements)
            {
                var token = el.ElementHandle.EntityToken;

                foreach (var provider in ElementActionProviders.Where(p => p.IsProviderFor(token)))
                {
                    provider.AddActions(el);
                }
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
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                    OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
                }
            };

            foreach (var provider in ElementActionProviders.Where(p => p.IsProviderFor(rootElement.ElementHandle.EntityToken)))
            {
                provider.AddActions(rootElement);
            }

            return new[] { rootElement };
        }

        public Dictionary<EntityToken, IEnumerable<EntityToken>> GetParents(IEnumerable<EntityToken> entityTokens)
        {
            var dictionary = new Dictionary<EntityToken, IEnumerable<EntityToken>>();

            foreach (var token in entityTokens)
            {
                var dataToken = token as DataEntityToken;
                if (dataToken == null)
                {
                    continue;
                }

                var modelReference = dataToken.Data as IModelReference;
                if (modelReference == null)
                {
                    continue;
                }

                var parts = modelReference.Name.Split(new[] { '.' });
                var ns = String.Join(".", parts.Take(parts.Length - 1));

                dictionary.Add(token, new[] { new NamespaceFolderEntityToken(typeof(FormBuilderElementProvider).Name, ns) });
            }

            return dictionary;
        }
    }
}
