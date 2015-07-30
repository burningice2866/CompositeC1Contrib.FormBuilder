using System.Collections.Generic;
using System.Collections.Specialized;

using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder
{
    public interface IModel
    {
        string Name { get; }
        IList<FormFieldModel> Fields { get; }

        bool DisableAntiForgery { get; }
        bool RequiresCaptcha { get; }
        bool ForceHttps { get; }
        bool HasFileUpload { get; }
    }
}
