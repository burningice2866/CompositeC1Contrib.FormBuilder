using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class DropdownInputElementAttribute : InputElementTypeAttribute
    {
        public override string ElementName
        {
            get { return "selectbox"; }
        }

        public override IHtmlString GetHtmlString(BaseFormBuilderRequestContext context, FormField field, IDictionary<string, string> htmlAttributes)
        {
            var sb = new StringBuilder();
            var renderer = context.FormRenderer;
            var htmlAttributesDictionary = MapHtmlTagAttributes(field, htmlAttributes);

            if (!String.IsNullOrEmpty(renderer.FormControlClass))
            {
                IList<string> list;
                if (!htmlAttributesDictionary.TryGetValue("class", out list))
                {
                    list = new List<string>();

                    htmlAttributesDictionary.Add("class", list);
                }

                list.Add(renderer.FormControlClass);
            }

            sb.AppendFormat("<select name=\"{0}\" id=\"{1}\"",
                        HttpUtility.HtmlAttributeEncode(field.Name),
                        HttpUtility.HtmlAttributeEncode(field.Id));

            if (field.ValueType == typeof(IEnumerable<string>))
            {
                htmlAttributesDictionary.Add("multiple", new List<string>() { "multiple" });
            }

            RenderExtraHtmlTags(sb, htmlAttributesDictionary);

            sb.Append(">");

            if (field.DataSource != null && field.DataSource.Any())
            {
                var value = field.Value;
                var selectLabel = renderer.HideLabels ? field.Label.Label : Localization.Widgets_Dropdown_SelectLabel;

                sb.AppendFormat("<option value=\"\" selected=\"selected\" disabled=\"disabled\">{0}</option>", HttpUtility.HtmlEncode(selectLabel));

                foreach (var item in field.DataSource)
                {
                    bool checkedValue;
                    if (field.ValueType == typeof(IEnumerable<string>))
                    {
                        checkedValue = ((IEnumerable<string>)field.Value ?? new String[] { }).Contains(item.Key);
                    }
                    else
                    {
                        checkedValue = item.Key == (value ?? String.Empty).ToString();
                    }

                    sb.AppendFormat("<option value=\"{0}\" {1}>{2}</option>",
                        HttpUtility.HtmlAttributeEncode(item.Key),
                        renderer.WriteChecked(checkedValue, "selected"),
                        HttpUtility.HtmlEncode(item.Value));
                }
            }

            sb.Append("</select>");

            return new HtmlString(sb.ToString());
        }
    }
}
