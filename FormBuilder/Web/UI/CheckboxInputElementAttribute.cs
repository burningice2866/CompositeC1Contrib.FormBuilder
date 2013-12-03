using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class CheckboxInputElementAttribute : InputElementTypeAttribute
    {
        public override string ElementName
        {
            get { return "checkbox"; }
        }

        public override IHtmlString GetHtmlString(FormField field, IDictionary<string, object> htmlAttributes)
        {
            var sb = new StringBuilder();
            var value = field.Value;

            if (field.ValueType == typeof(bool))
            {
                var bValue = false;
                try
                {
                    bValue = Convert.ToBoolean(value);
                }
                catch { }

                sb.AppendFormat("<input type=\"{0}\" name=\"{1}\" id=\"{2}\" value=\"on\" title=\"{3}\" {4}",
                    ElementName,
                    HttpUtility.HtmlAttributeEncode(field.Name),
                    HttpUtility.HtmlAttributeEncode(field.Id),
                    HttpUtility.HtmlAttributeEncode(field.Label.Label),
                    FormRenderer.WriteChecked(bValue, "checked"));

                FormRenderer.RenderReadOnlyAttribute(sb, field);
                FormRenderer.RenderExtraHtmlTags(sb, field, htmlAttributes);

                sb.Append(" />");
            }
            else if (field.ValueType == typeof(IEnumerable<string>))
            {
                var checkboxListOptions = field.DataSource;
                if (checkboxListOptions != null)
                {
                    var ix = 0;
                    IList<string> list;

                    if (value == null)
                    {
                        list = Enumerable.Empty<string>().ToList();
                    }
                    else
                    {
                        var str = value as string;
                        if (str != null)
                        {
                            list = new[] { str };
                        }
                        else
                        {
                            list = ((IEnumerable<string>)value).ToList();
                        }
                    }

                    foreach (var item in checkboxListOptions)
                    {
                        sb.Append("<label class=\"checkbox\">");

                        sb.AppendFormat("<input type=\"checkbox\" name=\"{1}\" id=\"{2}\" value=\"{3}\" title=\"{0}\" {4}",
                            HttpUtility.HtmlAttributeEncode(item.Value),
                            HttpUtility.HtmlAttributeEncode(field.Name),
                            HttpUtility.HtmlAttributeEncode(field.Id + "_" + ix++),
                            HttpUtility.HtmlAttributeEncode(item.Key),
                            FormRenderer.WriteChecked(list.Contains(item.Key), "checked"));

                        FormRenderer.RenderReadOnlyAttribute(sb, field);
                        FormRenderer.RenderMaxLengthAttribute(sb, field);
                        FormRenderer.RenderExtraHtmlTags(sb, field, htmlAttributes);

                        sb.AppendFormat("/> {0}</label>", HttpUtility.HtmlEncode(item.Value));
                    }
                }
            }

            return new HtmlString(sb.ToString());
        }
    }
}
