using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class FileuploadInputElement : IInputElementHandler
    {
        public string ElementName
        {
            get { return "file"; }
        }

        public virtual IHtmlString GetHtmlString(FormField field, IDictionary<string, object> htmlAttributes)
        {
            var sb = new StringBuilder();
            var htmlAttributesDictionary = FormRenderer.MapHtmlTagAttributes(field, htmlAttributes);

            sb.AppendFormat("<input type=\"{0}\" name=\"{1}\" id=\"{2}\"",
                        ElementName,
                        HttpUtility.HtmlAttributeEncode(field.Name),
                        HttpUtility.HtmlAttributeEncode(field.Id));

            if (field.ValueType == typeof(IEnumerable<FormFile>))
            {
                htmlAttributesDictionary.Add("multiple", new List<string>() { "multiple" });
            }

            var fileMimeTypeValidatorAttr = field.Attributes.OfType<FileMimeTypeValidatorAttribute>().SingleOrDefault();
            if (fileMimeTypeValidatorAttr != null)
            {
                htmlAttributesDictionary.Add("accept", fileMimeTypeValidatorAttr.MimeTypes);
            }
            
            FormRenderer.RenderExtraHtmlTags(sb, htmlAttributesDictionary);

            sb.Append(" />");

            return new HtmlString(sb.ToString());
        }
    }
}
