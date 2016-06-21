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

        public override IHtmlString GetHtmlString(BaseFormBuilderRequestContext context, FormField field, IDictionary<string, string> htmlAttributes)
        {
            var sb = new StringBuilder();
            var renderer = context.FormRenderer;
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
                    renderer.WriteChecked(bValue, "checked"));

                AddReadOnlyAttribute(field, htmlAttributes);
                RenderExtraHtmlTags(sb, field, htmlAttributes);

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
                        sb.AppendFormat("<label class=\"{0}\">", renderer.FormControlLabelClass(this));

                        sb.AppendFormat("<input type=\"checkbox\" name=\"{1}\" id=\"{2}\" value=\"{3}\" title=\"{0}\" {4}",
                            HttpUtility.HtmlAttributeEncode(item.Value),
                            HttpUtility.HtmlAttributeEncode(field.Name),
                            HttpUtility.HtmlAttributeEncode(field.Id + "_" + ix++),
                            HttpUtility.HtmlAttributeEncode(item.Key),
                            renderer.WriteChecked(list.Contains(item.Key), "checked"));

                        AddReadOnlyAttribute(field, htmlAttributes);
                        AddMaxLengthAttribute(field, htmlAttributes);
                        RenderExtraHtmlTags(sb, field, htmlAttributes);

                        sb.AppendFormat("/> {0}</label>", HttpUtility.HtmlEncode(item.Value));
                    }
                }
            }

            return new HtmlString(sb.ToString());
        }

        public override Type ResolveValueType(FormFieldModel field)
        {
            if (field.DataSource != null)
            {
                return typeof(IEnumerable<string>);
            }

            return typeof(bool);
        }
    }
}
