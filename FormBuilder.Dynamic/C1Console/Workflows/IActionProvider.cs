using Composite.C1Console.Elements;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public interface IActionPrivider
    {
        void AddActions(DynamicFormDefinition defintion, Element element);
    }
}
