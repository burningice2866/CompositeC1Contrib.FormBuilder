using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class PasswordInputElementAttribute : InputElementTypeAttribute
    {
        private const string _s = "<input type=\"{0}\" name=\"{1}\" id=\"{2}\" title=\"{3}\" placeholder=\"{4}\"";

        public override string ElementName
        {
            get { return "textbox"; }
        }

        public override IHtmlString GetHtmlString(FormField field, IDictionary<string, object> htmlAttributes)
        {
            var sb = new StringBuilder();
            var placeholderText = field.PlaceholderText;

            if (String.IsNullOrEmpty(placeholderText) && field.OwningForm.Options.HideLabels)
            {
                placeholderText = field.Label.Label;
            }

            sb.AppendFormat(_s,
                "password",
                HttpUtility.HtmlAttributeEncode(field.Name),
                HttpUtility.HtmlAttributeEncode(field.Id),
                HttpUtility.HtmlAttributeEncode(field.Label.Label),
                HttpUtility.HtmlAttributeEncode(placeholderText));

            FormRenderer.RenderReadOnlyAttribute(sb, field);
            FormRenderer.RenderMaxLengthAttribute(sb, field);
            FormRenderer.RenderExtraHtmlTags(sb, field, htmlAttributes);

            sb.Append(" />");

            return new HtmlString(sb.ToString());
        }
    }
}
