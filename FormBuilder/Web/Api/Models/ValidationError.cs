using System.Collections.Generic;

namespace CompositeC1Contrib.FormBuilder.Web.Api.Models
{
    public class ValidationError
    {
        public IEnumerable<string> AffectedFields { get; set; }
        public string Message { get; set; }
    }
}
