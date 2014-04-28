using System.Collections.Generic;

using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public interface IDynamicFormDefinition
    {
        string Name { get; }
        IList<FormSubmitHandler> SubmitHandlers { get; }
    }
}
