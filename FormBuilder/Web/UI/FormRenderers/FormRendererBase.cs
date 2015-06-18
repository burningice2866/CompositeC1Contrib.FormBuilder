using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using CompositeC1Contrib.FormBuilder.Web.Api.Models;

namespace CompositeC1Contrib.FormBuilder.Web.UI.FormRenderers
{
    public abstract class FormRendererBase : IFormFormRenderer
    {
        public abstract string ValidationSummaryClass { get; }
        public abstract string ErrorClass { get; }
        public abstract string ParentGroupClass { get; }
        public abstract string FieldGroupClass { get; }
        public abstract string HideLabelClass { get; }
        public abstract string FormControlClass { get; }

        public abstract string FormControlLabelClass(InputElementTypeAttribute inputElement);

        public virtual string ValidationSummary(IEnumerable<ValidationError> errors)
        {
            var sb = new StringBuilder();

            sb.Append("<div class=\"" + ValidationSummaryClass + "\">");
            
            if (!String.IsNullOrEmpty(Localization.Validation_ErrorNotification_Header))
            {
                sb.Append("<p>" + HttpUtility.HtmlEncode(Localization.Validation_ErrorNotification_Header) + "</p>");
            }

            sb.Append("<ul>");

            foreach (var itm in errors)
            {
                sb.Append("<li>" + itm.Message + "</li>");
            }

            sb.Append("</ul>");

            if (!String.IsNullOrEmpty(Localization.Validation_ErrorNotification_Footer))
            {
                sb.Append("<p>" + HttpUtility.HtmlEncode(Localization.Validation_ErrorNotification_Footer) + "</p>");
            }

            sb.Append("</div>");

            return sb.ToString();
        }
    }
}
