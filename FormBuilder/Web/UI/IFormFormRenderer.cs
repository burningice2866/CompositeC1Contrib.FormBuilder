using System.Collections.Generic;

using CompositeC1Contrib.FormBuilder.Web.Api.Models;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public interface IFormFormRenderer
    {
        string ValidationSummaryClass { get; }
        string ErrorClass { get; }
        string HideLabelClass { get; }
        string ParentGroupClass { get; }
        string FieldGroupClass { get; }
        string FormControlClass { get; }

        string FormControlLabelClass(InputElementTypeAttribute inputElement);
        string ValidationSummary(IEnumerable<ValidationError> errors);
    }
}
