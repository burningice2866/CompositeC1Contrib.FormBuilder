using System.Collections.Generic;

using Composite.Core.Xml;

using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public interface IDynamicFormDefinition
    {
        string Name { get; set; }
        IList<FormSubmitHandler> SubmitHandlers { get; }
        XhtmlDocument IntroText { get; set; }
        XhtmlDocument SuccessResponse { get; set; }
        IFormSettings Settings { get; set; }
    }
}
