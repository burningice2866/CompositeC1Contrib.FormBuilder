using System;
using System.Collections.Generic;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.C1Console.ElementProvider
{
    public interface IEntityTokenBasedElementProvider
    {
        Type EntityTokenType { get; }
        IEnumerable<Element> Handle(ElementProviderContext context, EntityToken token);
    }
}
