using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;
using Composite.Data;

using CompositeC1Contrib.FormBuilder.Data.Types;

namespace CompositeC1Contrib.FormBuilder.C1Console.EntityTokens
{
    [SecurityAncestorProvider(typeof(NamespaceFolderAncestorProvider))]
    public class NamespaceFolderEntityToken : EntityToken
    {
        public override string Type => String.Empty;

        public override string Source { get; }

        public override string Id => Namespace;

        public string Namespace { get; }

        public NamespaceFolderEntityToken(string source, string ns)
        {
            Source = source;
            Namespace = ns;
        }

        public static Element CreateElement(ElementProviderContext context, string source, string label, string ns)
        {
            using (var data = new DataConnection())
            {
                var folderHandle = context.CreateElementHandle(new NamespaceFolderEntityToken(source, ns));
                var folderElement = new Element(folderHandle)
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = label,
                        ToolTip = label,
                        HasChildren = data.Get<IModelReference>().Any(m => m.Name.StartsWith(ns + ".")),
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("datagroupinghelper-folder-closed"),
                        OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("datagroupinghelper-folder-open")
                    }
                };

                return folderElement;
            }
        }

        public override string Serialize()
        {
            return DoSerialize();
        }

        public static EntityToken Deserialize(string serializedEntityToken)
        {
            DoDeserialize(serializedEntityToken, out _, out var source, out var id);

            return new NamespaceFolderEntityToken(source, id);
        }
    }

    public class NamespaceFolderAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            if (!(entityToken is NamespaceFolderEntityToken namespaceToken))
            {
                return Enumerable.Empty<EntityToken>();
            }

            var split = namespaceToken.Namespace.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length == 1)
            {
                return new[] { new FormElementProviderEntityToken() };
            }

            var parentName = String.Join(".", split.Take(split.Length - 1));

            return new[] { new NamespaceFolderEntityToken(namespaceToken.Source, parentName) };
        }
    }
}
