using System.Collections.Generic;

namespace CompositeC1Contrib.FormBuilder.Web.Api.Models
{
    public class ValidationSummaryModel
    {
        public string Renderer { get; set; }
        public IEnumerable<ValidationError> Errors { get; set; }
    }
}
