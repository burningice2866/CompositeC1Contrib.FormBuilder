using System.Collections.Generic;

using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public interface IDynamicDefinition
    {
        string Name { get; set; }
        IList<FormSubmitHandler> SubmitHandlers { get; }
        IFormSettings Settings { get; set; }
    }
}
