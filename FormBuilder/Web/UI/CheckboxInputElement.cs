using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class CheckboxInputElement : IInputElementHandler
    {
        public string ElementName
        {
            get { return "checkbox"; }
        }

        public IHtmlString GetHtmlString(FormField field, IDictionary<string, object> htmlAttributes)
        {
            var sb = new StringBuilder();
            var value = field.Value;

            if (field.ValueType == typeof(bool))
            {
                sb.AppendFormat("<input type=\"{0}\" name=\"{1}\" id=\"{2}\" value=\"on\" title=\"{3}\" {4}",
                    ElementName,
                    HttpUtility.HtmlAttributeEncode(field.Name),
                    HttpUtility.HtmlAttributeEncode(field.Id),
                    HttpUtility.HtmlAttributeEncode(field.Label.Label),
                    FormRenderer.WriteChecked((bool)value, "checked"));

                FormRenderer.RenderExtraHtmlTags(sb, field, htmlAttributes);

                sb.Append(" />");
            }
            else if (field.ValueType == typeof(IEnumerable<string>))
            {
                var checkboxListOptions = field.DataSource;
                if (checkboxListOptions != null)
                {
                    var ix = 0;
                    var list = value == null ? Enumerable.Empty<string>() : (IEnumerable<string>)value;

                    foreach (var item in checkboxListOptions)
                    {
                        sb.Append("<label class=\"checkbox\">");

                        sb.AppendFormat("<input type=\"checkbox\" name=\"{1}\" id=\"{2}\" value=\"{3}\" title=\"{0}\" {4}",
                            HttpUtility.HtmlAttributeEncode(item.Value),
                            HttpUtility.HtmlAttributeEncode(field.Name),
                            HttpUtility.HtmlAttributeEncode(field.Id + "_" + ix++),
                            HttpUtility.HtmlAttributeEncode(item.Key),
                            FormRenderer.WriteChecked(list.Contains(item.Key), "checked"));

                        FormRenderer.RenderExtraHtmlTags(sb, field);

                        sb.AppendFormat("/> {0}</label>", HttpUtility.HtmlEncode(item.Value));
                    }
                }
            }

            return new HtmlString(sb.ToString());
        }
    }
}
