using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;

namespace CompositeC1Contrib.FormBuilder.C1Console.ElementProvider
{
    [Export("FormBuilder", typeof(IElementProviderFor))]
    public class NamespaceFolderEntityTokenHandler : IElementProviderFor
    {
        public IEnumerable<Type> ProviderFor => new[] { typeof(NamespaceFolderEntityToken) };

        public IEnumerable<Element> Provide(ElementProviderContext context, EntityToken token)
        {
            var folderToken = (NamespaceFolderEntityToken)token;
            var elements = FormElementProviderEntityTokenHandler.GetNamespaceAndFormElements(context, folderToken.Namespace);

            return elements;
        }
    }
}
