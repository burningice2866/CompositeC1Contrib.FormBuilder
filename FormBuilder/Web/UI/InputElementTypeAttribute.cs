using System;
using System.Collections.Generic;
using System.Web;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class InputElementTypeAttribute : Attribute
    {
        abstract public string ElementName { get; }

        protected InputElementTypeAttribute() { }

        abstract public IHtmlString GetHtmlString(FormField field, IDictionary<string, object> htmlAttributes);
    }
}
