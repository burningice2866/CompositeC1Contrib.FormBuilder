using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class InputElementTypeAttribute : Attribute
    {
        abstract public string ElementName { get; }

        abstract public IHtmlString GetHtmlString(FormField field, IDictionary<string, string> htmlAttributes);

        protected void RenderReadOnlyAttribute(StringBuilder sb, FormField field)
        {
            if (field.IsReadOnly)
            {
                sb.Append(" readonly=\"readonly\"");
            }
        }

        protected void RenderMaxLengthAttribute(StringBuilder sb, FormField field)
        {
            var maxLengthAttribute = field.ValidationAttributes.OfType<MaxFieldLengthAttribute>().FirstOrDefault();
            if (maxLengthAttribute == null)
            {
                return;
            }

            sb.AppendFormat(" maxlength=\"{0}\"", maxLengthAttribute.Length);
        }

        protected void AddHtmlAttribute(string attribute, string value, IDictionary<string, string> htmlAttributes)
        {
            string currValue;
            if (!htmlAttributes.TryGetValue(attribute, out currValue))
            {
                htmlAttributes.Add(attribute, value);
            }
            else
            {
                htmlAttributes[attribute] += " " + value;
            }
        }

        protected void RenderExtraHtmlTags(StringBuilder sb, FormField field, IDictionary<string, string> htmlAttributes)
        {
            var htmlAttributesDictionary = MapHtmlTagAttributes(field, htmlAttributes);

            RenderExtraHtmlTags(sb, htmlAttributesDictionary);
        }

        protected void RenderExtraHtmlTags(StringBuilder sb, Dictionary<string, IList<string>> htmlAttributesDictionary)
        {
            foreach (var kvp in htmlAttributesDictionary)
            {
                sb.Append(" " + kvp.Key + "=\"");

                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    var itm = kvp.Value[i];

                    sb.Append(itm);

                    if ((i + 1) < kvp.Value.Count)
                    {
                        var seperator = kvp.Key == "accept" ? "," : " ";

                        sb.Append(seperator);
                    }
                }

                sb.Append("\"");
            }
        }

        protected Dictionary<string, IList<string>> MapHtmlTagAttributes(FormField field, IDictionary<string, string> htmlAttributes)
        {
            var htmlAttributesDictionary = new Dictionary<string, IList<string>>()
            {
                { "class", new List<string>() }
            };

            var htmlElementAttributes = field.Attributes.OfType<HtmlTagAttribute>();

            foreach (var attr in htmlElementAttributes)
            {
                IList<string> list;
                if (!htmlAttributesDictionary.TryGetValue(attr.Attribute, out list))
                {
                    htmlAttributesDictionary.Add(attr.Attribute, new List<string>());
                }

                htmlAttributesDictionary[attr.Attribute].Add(attr.Value);
            }

            if (htmlAttributes != null && htmlAttributes.ContainsKey("class"))
            {
                var list = htmlAttributesDictionary["class"];
                var val = htmlAttributes["class"].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var itm in val)
                {
                    list.Add(itm);
                }
            }

            return htmlAttributesDictionary;
        }
    }
}
