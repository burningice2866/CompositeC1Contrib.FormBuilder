using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class RadioButtonInputElement : IInputElementHandler
    {
        public string ElementName
        {
            get { return "radio"; }
        }

        public IHtmlString GetHtmlString(FormField field, IDictionary<string, object> htmlAttributes)
        {
            var sb = new StringBuilder();

            if (field.DataSource != null && field.DataSource.Any())
            {
                var ix = 0;
                var value = field.Value;
                var htmlAttributesDictionary = FormRenderer.MapHtmlTagAttributes(field, htmlAttributes);

                foreach (var item in field.DataSource)
                {
                    sb.AppendFormat("<label class=\"{0}\">", ElementName);

                    sb.AppendFormat("<input type=\"{0}\" name=\"{2}\" id=\"{3}\" value=\"{4}\" title=\"{1}\" {5}",
                        ElementName,
                        HttpUtility.HtmlAttributeEncode(item.Value),
                        HttpUtility.HtmlAttributeEncode(field.Name),
                        HttpUtility.HtmlAttributeEncode(field.Id + "_" + ix++),
                        HttpUtility.HtmlAttributeEncode(item.Key),
                        (value == null ? String.Empty : FormRenderer.WriteChecked(FormRenderer.IsEqual(value, item.Key), "checked")));

                    FormRenderer.RenderExtraHtmlTags(sb, htmlAttributesDictionary);

                    sb.AppendFormat(" /> {0}</label>", HttpUtility.HtmlEncode(item.Value));
                }
            }

            return new HtmlString(sb.ToString());
        }
    }
}
