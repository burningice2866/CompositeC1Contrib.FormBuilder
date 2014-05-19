using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;

using CompositeC1Contrib.FormBuilder.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider
{
    [Export("FormBuilder.Dynamic", typeof(IEntityTokenBasedElementProvider))]
    public class NamespaceFolderEntityTokenHandler : IEntityTokenBasedElementProvider
    {
        public Type EntityTokenType
        {
            get { return typeof(NamespaceFolderEntityToken); }
        }

        public IEnumerable<Element> Handle(ElementProviderContext context, EntityToken token)
        {
            var folderToken = (NamespaceFolderEntityToken) token;
            var elements = FormElementProviderEntityTokenHandler.GetNamespaceAndFormElements(context, folderToken.Namespace);

            return elements;
        }
    }
}
