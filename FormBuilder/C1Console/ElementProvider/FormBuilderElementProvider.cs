using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;
using Composite.Data;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Data.Types;

namespace CompositeC1Contrib.FormBuilder.C1Console.ElementProvider
{
    public class FormBuilderElementProvider : HooklessElementProvider, IAuxiliarySecurityAncestorProvider
    {
        public FormBuilderElementProvider() : base("FormBuilder") { }

        public override IEnumerable<Element> GetRootsImpl(SearchToken searchToken)
        {
            var elementHandle = Context.CreateElementHandle(new FormElementProviderEntityToken());
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
