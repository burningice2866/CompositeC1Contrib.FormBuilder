namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public interface IFormSettings
    {
        string GetFormExecutor(IDynamicFormDefinition definition);
    }
}
