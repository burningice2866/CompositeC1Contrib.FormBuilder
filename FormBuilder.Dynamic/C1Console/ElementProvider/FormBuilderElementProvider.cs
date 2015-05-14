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
using Composite.Data;

using CompositeC1Contrib.FormBuilder.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Data.Types;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider
{
    public class FormBuilderElementProvider : IHooklessElementProvider, IAuxiliarySecurityAncestorProvider
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup(ActionGroupPriority.PrimaryHigh);
        private static readonly IEnumerable<PermissionType> AddPermissions = new[] { PermissionType.Add };

        private static readonly IDictionary<Type, IEntityTokenBasedElementProvider> EntityTokenHandlers = new Dictionary<Type, IEntityTokenBasedElementProvider>();

        public static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = true, ActionGroup = ActionGroup };

        private ElementProviderContext _context;
        public ElementProviderContext Context
        {
            set { _context = value; }
        }

        static FormBuilderElementProvider()
        {
            var batch = new CompositionBatch();
            var catalog = new SafeDirectoryCatalog(HttpRuntime.BinDirectory);
            var container = new CompositionContainer(catalog);

            container.Compose(batch);

            var any = container.GetExportedValues<IEntityTokenBasedElementProvider>().ToDictionary(o => o.EntityTokenType, o => o);

            EntityTokenHandlers = any.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
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

            ConfigureFolderActions(rootElement);

            return new[] { rootElement };
        }

        public static void ConfigureFolderActions(Element element)
        {
            var addFormActionToken = new WorkflowActionToken(typeof(AddFormWorkflow), AddPermissions);
            element.AddAction(new ElementAction(new ActionHandle(addFormActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Add form",
                    ToolTip = "Add form",
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                    ActionLocation = ActionLocation
                }
            });

            var addWizardActionToken = new WorkflowActionToken(typeof(AddFormWizardWorkflow), AddPermissions);
            element.AddAction(new ElementAction(new ActionHandle(addWizardActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Add wizard",
                    ToolTip = "Add wizard",
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                    ActionLocation = ActionLocation
                }
            });
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

                var form = dataToken.Data as IForm;
                if (form == null)
                {
                    continue;
                }

                var parts = form.Name.Split(new[] { '.' });
                var ns = String.Join(".", parts.Take(parts.Length - 1));

                dictionary.Add(token, new[] { new NamespaceFolderEntityToken(typeof(FormBuilderElementProvider).Name, ns) });
            }

            return dictionary;
        }
    }
}
