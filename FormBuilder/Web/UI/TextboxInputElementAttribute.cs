﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class TextboxInputElementAttribute : InputElementTypeAttribute
    {
        private const string Markup = "<input type=\"{0}\" name=\"{1}\" id=\"{2}\" value=\"{3}\" title=\"{4}\" placeholder=\"{5}\"";

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
                EvaluateTextboxType(field),
                HttpUtility.HtmlAttributeEncode(field.Name),
                HttpUtility.HtmlAttributeEncode(field.Id),
                field.Value == null ? String.Empty : HttpUtility.HtmlAttributeEncode(field.GetValueAsString()),
                HttpUtility.HtmlAttributeEncode(field.Label),
                HttpUtility.HtmlAttributeEncode(placeholderText));

            AddHtmlAttribute("class", renderer.FormControlClass, htmlAttributes);

            AddReadOnlyAttribute(field, htmlAttributes);
            AddMaxLengthAttribute(field, htmlAttributes);
            RenderExtraHtmlTags(sb, field, htmlAttributes);

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
