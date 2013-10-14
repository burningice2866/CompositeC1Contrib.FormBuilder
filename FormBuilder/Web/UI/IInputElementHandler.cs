using System.Collections.Generic;
using System.Web;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public interface IInputElementHandler
    {
        string ElementName { get; }
        IHtmlString GetHtmlString(FormField field, IDictionary<string, object> htmlAttributes);
    }
}
