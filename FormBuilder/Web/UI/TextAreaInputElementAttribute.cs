using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class TextAreaInputElementAttribute : InputElementTypeAttribute
    {
        private const string Markup = "<textarea name=\"{0}\" id=\"{1}\" title=\"{2}\" placeholder=\"{2}\" rows=\"{3}\" cols=\"{4}\"";

        public int Cols { get; set; }
        public int Rows { get; set; }

        public override string ElementName
        {
            get { return "textarea"; }
        }

        public TextAreaInputElementAttribute()
        {
            Cols = 40;
            Rows = 5;
        }

        public TextAreaInputElementAttribute(int cols, int rows)
        {
            Cols = cols;
            Rows = rows;
        }

        public override IHtmlString GetHtmlString(BaseFormBuilderRequestContext context, FormField field, IDictionary<string, string> htmlAttributes)
        {
            var sb = new StringBuilder();
            var placeholderText = field.PlaceholderText;

            if (String.IsNullOrEmpty(placeholderText) && context.FormRenderer.HideLabels)
            {
                placeholderText = field.Label.Label;
            }

            sb.AppendFormat(Markup,
                HttpUtility.HtmlAttributeEncode(field.Name),
                HttpUtility.HtmlAttributeEncode(field.Id),
                HttpUtility.HtmlAttributeEncode(placeholderText),
                Rows,
                Cols);

            AddHtmlAttribute("class", context.FormRenderer.FormControlClass, htmlAttributes);

            AddReadOnlyAttribute(field, htmlAttributes);
            AddMaxLengthAttribute(field, htmlAttributes);
            RenderExtraHtmlTags(sb, field, htmlAttributes);

            sb.AppendFormat(">{0}</textarea>", HttpUtility.HtmlEncode(field.GetValueAsString()));

            return new HtmlString(sb.ToString());
        }
    }
}
