using System.Collections.Generic;
using System.Collections.Specialized;

using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder
{
    public interface IFormModel
    {
        string Name { get; }
        IList<FormValidationRule> ValidationResult { get; }
        IList<FormField> Fields { get; }

        bool ForceHttps { get; }
        bool HasFileUpload { get; }

        void SetDefaultValues();
        void MapValues(NameValueCollection values, IEnumerable<FormFile> files);
        void Validate();
    }
}
