using Composite.C1Console.Elements;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public interface IActionPrivider
    {
        void AddActions(IDynamicFormDefinition defintion, Element element);
    }
}
