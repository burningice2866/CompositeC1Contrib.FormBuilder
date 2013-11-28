using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class TextboxInputElement : IInputElementHandler
    {
        private const string _s = "<input type=\"{0}\" name=\"{1}\" id=\"{2}\" value=\"{3}\" title=\"{4}\" placeholder=\"{5}\"";

        public string ElementName
        {
            get { return "textbox"; }
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
                EvaluateTextboxType(field),
                HttpUtility.HtmlAttributeEncode(field.Name),
                HttpUtility.HtmlAttributeEncode(field.Id),
                field.Value == null ? String.Empty : HttpUtility.HtmlAttributeEncode(FormRenderer.GetValue(field)),
                HttpUtility.HtmlAttributeEncode(field.Label.Label),
                HttpUtility.HtmlAttributeEncode(placeholderText));

            FormRenderer.RenderMaxLengthAttribute(sb, field);
            FormRenderer.RenderExtraHtmlTags(sb, field, htmlAttributes);

            sb.Append(" />");

            return new HtmlString(sb.ToString());
        }

        private static string EvaluateTextboxType(FormField field)
        {
            var type = Nullable.GetUnderlyingType(field.ValueType) ?? field.ValueType;

            if (type == typeof(DateTime))
            {
                return "date";
            }

            if (type == typeof(int))
            {
                return "number";
            }

            if (type == typeof(string))
            {
                if (field.ValidationAttributes.Any(f => f is EmailFieldValidatorAttribute))
                {
                    return "email";
                }
            }

            return "text";
        }
    }
}
