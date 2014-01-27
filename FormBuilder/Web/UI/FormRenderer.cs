using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

using Composite.Core.ResourceSystem;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Configuration;
using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public static class FormRenderer
    {
        public static readonly IFormFormRenderer RendererImplementation;

        static FormRenderer()
        {
            var type = FormBuilderConfiguration.GetSection().RendererImplementation;

            RendererImplementation = (IFormFormRenderer)Activator.CreateInstance(type);
        }

        public static IHtmlString FieldFor(FormField field)
        {
            return WriteRow(field, new Dictionary<string, string>());
        }

        public static IHtmlString NameFor(FormField field)
        {
            return new HtmlString(field.Name);
        }

        public static IHtmlString WriteErrors(FormModel model)
        {
            var sb = new StringBuilder();
            var validationResult = model.ValidationResult;

            if (validationResult.Any())
            {
                sb.Append("<div class=\"" + RendererImplementation.ErrorNotificationClass + "\">");

                if (!String.IsNullOrEmpty(Localization.Validation_ErrorNotificationTop))
                {
                    sb.Append("<p>" + HttpUtility.HtmlEncode(Localization.Validation_ErrorNotificationTop) + "</p>");
                }

                sb.Append("<ul>");

                foreach (var el in validationResult)
                {
                    sb.Append("<li>" + el.ValidationMessage + "</li>");
                }

                sb.Append("</ul>");

                if (!String.IsNullOrEmpty(Localization.Validation_ErrorNotificationBottom))
                {
                    sb.Append("<p>" + HttpUtility.HtmlEncode(Localization.Validation_ErrorNotificationBottom) + "</p>");
                }

                sb.Append("</div>");
            }

            return new HtmlString(sb.ToString());
        }

        private static IHtmlString WriteRow(FormField field, IDictionary<string, string> htmlAttributes)
        {
            var sb = new StringBuilder();
            var includeLabel = ShowLabel(field);
            var validationResult = field.OwningForm.ValidationResult;

            sb.AppendFormat("<div id=\"form-field-{0}\" class=\"" + RendererImplementation.ParentGroupClass + "-group control-{1} {2} {3} \"", field.Name, field.InputElementType.ElementName, WriteErrorClass(field.Name, validationResult), field.IsRequired ? "required" : String.Empty);

            DependencyAttributeFor(field, sb);

            sb.Append(">");

            if (!(field.InputElementType is CheckboxInputElementAttribute && field.ValueType == typeof(bool)))
            {
                if (includeLabel)
                {
                    WriteLabel(field, sb);
                }
                else
                {
                    WritePropertyHeading(field, sb);
                }
            }

            sb.Append("<div class=\"" + RendererImplementation.FieldGroupClass + "\">");

            if (field.InputElementType is CheckboxInputElementAttribute && field.ValueType == typeof(bool))
            {
                sb.Append("<label class=\"checkbox\">");
            }

            WriteField(field, sb, htmlAttributes);

            if (field.InputElementType is CheckboxInputElementAttribute && field.ValueType == typeof(bool))
            {
                WriteLabelContent(field, sb);

                sb.Append("</label>");
            }

            sb.Append("</div></div>");

            return new HtmlString(sb.ToString());
        }

        private static bool ShowLabel(FormField field)
        {
            if (field.ValueType == typeof(bool))
            {
                return true;
            }

            if (field.InputElementType is CheckboxInputElementAttribute || field.InputElementType is RadioButtonInputElementAttribute)
            {
                return false;
            }

            return true;
        }

        private static void WriteField(FormField field, StringBuilder sb, IDictionary<string, string> htmlAttributes)
        {
            var str = field.InputElementType.GetHtmlString(field, htmlAttributes);

            if (!String.IsNullOrWhiteSpace(field.Help))
            {
                sb.Append("<div class=\"input-append\">");
            }

            sb.Append(str);

            if (!String.IsNullOrWhiteSpace(field.Help))
            {
                sb.Append("<div class=\"info-block\">");
                sb.Append("<span class=\"add-on info-icon\">i</span>");
                sb.AppendFormat("<div class=\"info-msg\">{0}</div>", field.Help);
                sb.Append("</div>");
                sb.Append("</div>");
            }
        }

        public static void DependencyAttributeFor(FormField field, StringBuilder sb)
        {
            if (field.DependencyAttributes.Any())
            {
                var dependencyObj = new StringBuilder();
                var attrs = field.DependencyAttributes.ToList();

                dependencyObj.Append("[ ");

                for (int i = 0; i < attrs.Count; i++)
                {
                    var dependencyAttribute = attrs[i];
                    var values = dependencyAttribute.RequiredFieldValues();

                    dependencyObj.Append("{ &quot;field&quot;: &quot;" + dependencyAttribute.ReadFromFieldName + "&quot;, &quot;value&quot;:");

                    dependencyObj.Append("[ ");

                    for (int j = 0; j < values.Length; j++)
                    {
                        dependencyObj.Append("&quot;" + values[j] + "&quot;");

                        if (j < (values.Length - 1))
                        {
                            dependencyObj.Append(",");
                        }
                    }

                    dependencyObj.Append(" ]");

                    dependencyObj.Append(" }");

                    if (i < (attrs.Count - 1))
                    {
                        dependencyObj.Append(", ");
                    }
                }

                dependencyObj.Append(" ]");

                sb.AppendFormat(" data-dependency=\"{0}\"", dependencyObj);
            }
        }

        public static string WriteChecked(bool write, string attr)
        {
            return write ? String.Format("{0}=\"{0}\"", attr) : String.Empty;
        }

        public static bool IsEqual(object obj, string value)
        {
            if (obj is bool)
            {
                return bool.Parse(value) == (bool)obj;
            }

            return obj.ToString() == value;
        }

        private static void WriteLabel(FormField field, StringBuilder sb)
        {
            var hide = field.OwningForm.Options.HideLabels;
            if (field.InputElementType is FileuploadInputElementAttribute)
            {
                hide = false;
            }

            sb.AppendFormat("<label class=\"control-label {0}\" for=\"{1}\">", hide ? "hide-text " : String.Empty, field.Id);

            WriteLabelContent(field, sb);

            sb.Append("</label>");
        }

        private static void WritePropertyHeading(FormField field, StringBuilder sb)
        {
            sb.Append("<p class=\"control-label\">");

            WriteLabelContent(field, sb);

            sb.Append("</p>");
        }

        private static void WriteLabelContent(FormField field, StringBuilder sb)
        {
            if (field.IsRequired)
            {
                sb.Append("<span class=\"required\">*</span>");
            }

            if (!String.IsNullOrEmpty(field.Label.Link))
            {
                sb.AppendFormat("<a href=\"{0}\" title=\"{1}\" {2}>{3}</a>",
                    HttpUtility.HtmlAttributeEncode(field.Label.Link),
                    HttpUtility.HtmlAttributeEncode(field.Label.Label),
                    field.Label.OpenLinkInNewWindow ? "target=\"_blank\"" : String.Empty,
                    HttpUtility.HtmlEncode(field.Label.Label));
            }
            else
            {
                sb.Append(HttpUtility.HtmlEncode(field.Label.Label));
            }
        }

        public static string WriteErrorClass(string name, IEnumerable<FormValidationRule> validationResult)
        {
            return validationResult.Any(el => el.AffectedFormIds.Contains(name)) ? RendererImplementation.ErrorClass : String.Empty;
        }

        public static string GetValue(FormField field)
        {
            if (field.Value == null)
            {
                return String.Empty;
            }

            var formatAttr = field.Attributes.OfType<DisplayFormatAttribute>().SingleOrDefault();

            var underlyingType = Nullable.GetUnderlyingType(field.ValueType);
            if (underlyingType != null && formatAttr != null)
            {
                var hasValue = (bool)field.ValueType.GetProperty("HasValue").GetValue(field.Value, null);
                if (hasValue)
                {
                    var value = field.ValueType.GetProperty("Value").GetValue(field.Value, null) as IFormattable;
                    if (value != null)
                    {
                        return value.ToString(formatAttr.FormatString, CultureInfo.CurrentUICulture);
                    }
                }
            }

            if (formatAttr == null || !(field.Value is IFormattable))
            {
                return field.Value.ToString();
            }

            return ((IFormattable)field.Value).ToString(formatAttr.FormatString, CultureInfo.CurrentUICulture);
        }

        public static string GetLocalized(string text)
        {
            return text.Contains("${") ? StringResourceSystemFacade.ParseString(text) : text;
        }
    }
}
