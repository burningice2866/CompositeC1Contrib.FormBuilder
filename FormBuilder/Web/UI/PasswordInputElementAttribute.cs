using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class PasswordInputElementAttribute : InputElementTypeAttribute
    {
        private const string Markup = "<input type=\"{0}\" name=\"{1}\" id=\"{2}\" title=\"{3}\" placeholder=\"{4}\"";

        public override string ElementName => "textbox";

        public override IHtmlString GetHtmlString(BaseFormBuilderRequestContext context, FormField field, IDictionary<string, string> htmlAttributes)
        {
            var sb = new StringBuilder();
            var renderer = context.FormRenderer;
            var placeholderText = field.PlaceholderText;

            if (String.IsNullOrEmpty(placeholderText) && renderer.HideLabels)
            {
                placeholderText = field.Label;
            }

            sb.AppendFormat(Markup,
                "password",
                HttpUtility.HtmlAttributeEncode(field.Name),
                HttpUtility.HtmlAttributeEncode(field.Id),
                HttpUtility.HtmlAttributeEncode(field.Label),
                HttpUtility.HtmlAttributeEncode(placeholderText));

            AddHtmlAttribute("class", renderer.FormControlClass, htmlAttributes);

            AddReadOnlyAttribute(field, htmlAttributes);
            AddMaxLengthAttribute(field, htmlAttributes);
            RenderExtraHtmlTags(sb, field, htmlAttributes);

            sb.Append(" />");

            return new HtmlString(sb.ToString());
        }
    }
}
