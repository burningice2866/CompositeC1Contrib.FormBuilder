﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class RadioButtonInputElementAttribute : InputElementTypeAttribute
    {
        public override string ElementName => "radio";

        public override IHtmlString GetHtmlString(BaseFormBuilderRequestContext context, FormField field, IDictionary<string, string> htmlAttributes)
        {
            var sb = new StringBuilder();
            var renderer = context.FormRenderer;

            if (field.DataSource != null && field.DataSource.Any())
            {
                var ix = 0;
                var value = field.Value;
                var htmlAttributesDictionary = MapHtmlTagAttributes(field, htmlAttributes);

                foreach (var item in field.DataSource)
                {
                    sb.AppendFormat("<label class=\"{0}\">", renderer.FormControlLabelClass(this));

                    sb.AppendFormat("<input type=\"{0}\" name=\"{2}\" id=\"{3}\" value=\"{4}\" title=\"{1}\" {5}",
                        ElementName,
                        HttpUtility.HtmlAttributeEncode(item.Value),
                        HttpUtility.HtmlAttributeEncode(field.Name),
                        HttpUtility.HtmlAttributeEncode(field.Id + "_" + ix++),
                        HttpUtility.HtmlAttributeEncode(item.Key),
                        (value == null ? String.Empty : renderer.WriteChecked(item.Key.IsEqualTo(value), "checked")));

                    AddReadOnlyAttribute(field, htmlAttributes);
                    RenderExtraHtmlTags(sb, htmlAttributesDictionary);

                    sb.AppendFormat(" /> {0}</label>", HttpUtility.HtmlEncode(item.Value));
                }
            }

            return new HtmlString(sb.ToString());
        }
    }
}
