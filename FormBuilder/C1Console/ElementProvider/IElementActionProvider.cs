using Composite.C1Console.Elements;
using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.C1Console.ElementProvider
{
    public interface IElementActionProvider
    {
        bool IsProviderFor(EntityToken entityToken);
        void AddActions(Element element);
    }
}
