using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class TextAreaInputElement : IInputElementHandler
    {
        private const string _s = "<textarea name=\"{0}\" id=\"{1}\" rows=\"5\" cols=\"40\" title=\"{2}\" placeholder=\"{2}\"";

        public string ElementName
        {
            get { return "textarea"; }
        }

        public IHtmlString GetHtmlString(FormField field, IDictionary<string, object> htmlAttributes)
        {
            var sb = new StringBuilder();
            var placeholderText = field.PlaceholderText;

            if (String.IsNullOrEmpty(placeholderText) && field.OwningForm.Options.HideLabels)
            {
                placeholderText = field.Label.Label;
            }

            sb.AppendFormat(_s,
                HttpUtility.HtmlAttributeEncode(field.Name),
                HttpUtility.HtmlAttributeEncode(field.Id),
                HttpUtility.HtmlAttributeEncode(placeholderText));

            FormRenderer.RenderMaxLengthAttribute(sb, field);
            FormRenderer.RenderExtraHtmlTags(sb, field, htmlAttributes);

            sb.AppendFormat(">{0}</textarea>", HttpUtility.HtmlEncode(FormRenderer.GetValue(field)));

            return new HtmlString(sb.ToString());
        }
    }
}
