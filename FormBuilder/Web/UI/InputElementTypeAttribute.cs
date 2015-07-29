using System;
using System.Collections.Generic;
using System.Globalization;
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

        abstract public IHtmlString GetHtmlString(BaseFormBuilderRequestContext context, FormField field, IDictionary<string, string> htmlAttributes);

        protected void AddReadOnlyAttribute(FormField field, IDictionary<string, string> htmlAttributes)
        {
            if (field.IsReadOnly)
            {
                AddHtmlAttribute("readonly", "readonly", htmlAttributes);
            }
        }

        protected void AddMaxLengthAttribute(FormField field, IDictionary<string, string> htmlAttributes)
        {
            var maxLengthAttribute = field.ValidationAttributes.OfType<MaxFieldLengthAttribute>().FirstOrDefault();
            if (maxLengthAttribute != null)
            {
                AddHtmlAttribute("maxlength", maxLengthAttribute.Length.ToString(CultureInfo.InvariantCulture), htmlAttributes);
            }
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
            var result = new Dictionary<string, IList<string>>();

            var fieldHmlTagAttributes = field.Attributes.OfType<HtmlTagAttribute>();
            foreach (var attr in fieldHmlTagAttributes)
            {
                IList<string> list;
                if (!result.TryGetValue(attr.Attribute, out list))
                {
                    result.Add(attr.Attribute, new List<string>());
                }

                result[attr.Attribute].Add(attr.Value);
            }

            if (htmlAttributes != null)
            {
                foreach (var kvp in htmlAttributes)
                {
                    IList<string> list;
                    if (!result.TryGetValue(kvp.Key, out list))
                    {
                        list = new List<string>();

                        result.Add(kvp.Key, list);
                    }

                    var val = kvp.Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var itm in val)
                    {
                        list.Add(itm);
                    }
                }
            }

            return result;
        }
    }
}
