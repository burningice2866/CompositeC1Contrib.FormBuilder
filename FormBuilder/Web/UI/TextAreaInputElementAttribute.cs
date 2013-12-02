using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class TextAreaInputElementAttribute : InputElementTypeAttribute
    {
        private const string _s = "<textarea name=\"{0}\" id=\"{1}\" title=\"{2}\" placeholder=\"{2}\" rows=\"{3}\" cols=\"{4}\"";

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

        public override IHtmlString GetHtmlString(FormField field, IDictionary<string, object> htmlAttributes)
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
                HttpUtility.HtmlAttributeEncode(placeholderText),
                Rows,
                Cols);

            FormRenderer.RenderMaxLengthAttribute(sb, field);
            FormRenderer.RenderExtraHtmlTags(sb, field, htmlAttributes);

            sb.AppendFormat(">{0}</textarea>", HttpUtility.HtmlEncode(FormRenderer.GetValue(field)));

            return new HtmlString(sb.ToString());
        }
    }
}
