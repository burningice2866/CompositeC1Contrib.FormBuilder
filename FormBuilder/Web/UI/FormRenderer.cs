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

        public static IHtmlString FieldFor(FormOptions options, FormField field)
        {
            return FieldFor(options, field, new Dictionary<string, string>());
        }

        public static IHtmlString FieldFor(FormOptions options, FormField field, Dictionary<string, string> htmlAttributes)
        {
            return WriteRow(options, field, htmlAttributes);
        }

        public static IHtmlString InputFor(FormOptions options, FormField field, Dictionary<string, string> htmlAttributes)
        {
            var sb = new StringBuilder();

            WriteField(options, field, sb, htmlAttributes);

            return new HtmlString(sb.ToString());
        }

        public static IHtmlString NameFor(FormField field)
        {
            return new HtmlString(field.Name);
        }

        public static IHtmlString WriteErrors(IFormModel model)
        {
            var validationResult = model.ValidationResult;

            return WriteErrors(validationResult);
        }

        public static IHtmlString WriteErrors(ValidationResultList validationResult)
        {
            if (!validationResult.Any())
            {
                return new HtmlString(String.Empty);
            }

            var sb = new StringBuilder();

            sb.Append("<div class=\"" + RendererImplementation.ErrorNotificationClass + "\">");

            if (!String.IsNullOrEmpty(Localization.Validation_ErrorNotification_Header))
            {
                sb.Append("<p>" + HttpUtility.HtmlEncode(Localization.Validation_ErrorNotification_Header) + "</p>");
            }

            sb.Append("<ul>");

            foreach (var el in validationResult)
            {
                sb.Append("<li>" + el.ValidationMessage + "</li>");
            }

            sb.Append("</ul>");

            if (!String.IsNullOrEmpty(Localization.Validation_ErrorNotification_Footer))
            {
                sb.Append("<p>" + HttpUtility.HtmlEncode(Localization.Validation_ErrorNotification_Footer) + "</p>");
            }

            sb.Append("</div>");

            return new HtmlString(sb.ToString());
        }

        private static IHtmlString WriteRow(FormOptions options, FormField field, IDictionary<string, string> htmlAttributes)
        {
            var fieldsRow = FieldsRow.Current;

            var sb = new StringBuilder();
            var includeLabel = ShowLabel(field);
            var validationResult = field.OwningForm.ValidationResult;

            WriteRowStart(field.Name, field.InputElementType.ElementName,
                WriteErrorClass(field.Name, validationResult), field.IsRequired,
                builder => DependencyAttributeFor(field, builder), sb);

            if (fieldsRow == null || fieldsRow.IncludeLabels)
            {
                if (!(field.InputElementType is CheckboxInputElementAttribute && field.ValueType == typeof(bool)))
                {
                    if (includeLabel)
                    {
                        WriteLabel(options, field, sb);
                    }
                    else
                    {
                        WritePropertyHeading(field, sb);
                    }
                }
            }

            using (new ControlsGroup(sb))
            {
                if (fieldsRow == null || fieldsRow.IncludeLabels)
                {
                    if (field.InputElementType is CheckboxInputElementAttribute && field.ValueType == typeof(bool))
                    {
                        sb.Append("<label class=\"checkbox\">");
                    }
                }

                WriteField(options, field, sb, htmlAttributes);

                if (fieldsRow == null || fieldsRow.IncludeLabels)
                {
                    if (field.InputElementType is CheckboxInputElementAttribute && field.ValueType == typeof(bool))
                    {
                        WriteLabelContent(field, sb);

                        sb.Append("</label>");
                    }
                }
            }

            WriteRowEnd(sb);

            return new HtmlString(sb.ToString());
        }

        public static void WriteRowStart(string name, string elementName, string errorClass, bool isRequired, Action<StringBuilder> extraAttributesRenderer, StringBuilder sb)
        {
            if (FieldsRow.Current != null)
            {
                return;
            }

            sb.AppendFormat("<div id=\"form-field-{0}\" class=\"" + RendererImplementation.ParentGroupClass + "-group control-{1} {2} {3} \"", name, elementName, errorClass, isRequired ? "required" : String.Empty);

            if (extraAttributesRenderer != null)
            {
                extraAttributesRenderer(sb);
            }

            sb.Append(">");
        }

        public static void WriteRowEnd(StringBuilder sb)
        {
            if (FieldsRow.Current != null)
            {
                return;
            }

            sb.Append("</div>");
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

        public static void WriteFieldHelpStart(StringBuilder sb)
        {
            sb.Append("<div class=\"input-append\">");
        }

        public static void WriteFieldHelpEnd(string help, StringBuilder sb)
        {
            sb.Append("<div class=\"info-block\">");
            sb.Append("<span class=\"add-on info-icon\">i</span>");
            sb.AppendFormat("<div class=\"info-msg\">{0}</div>", help);
            sb.Append("</div>");
            sb.Append("</div>");
        }

        private static void WriteField(FormOptions options, FormField field, StringBuilder sb, IDictionary<string, string> htmlAttributes)
        {
            var str = field.InputElementType.GetHtmlString(options, field, htmlAttributes);

            if (!String.IsNullOrWhiteSpace(field.Help))
            {
                WriteFieldHelpStart(sb);
            }

            sb.Append(str);

            if (!String.IsNullOrWhiteSpace(field.Help))
            {
                WriteFieldHelpEnd(field.Help, sb);
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

        public static void WriteLabelStart(bool hide, string id, StringBuilder sb)
        {
            sb.AppendFormat("<label class=\"control-label {0}\" for=\"{1}\">", hide ? "hide-text " : String.Empty, id);
        }

        public static void WriteLabelEnd(StringBuilder sb)
        {
            sb.Append("</label>");
        }

        private static void WriteLabel(FormOptions options, FormField field, StringBuilder sb)
        {
            var hide = options.HideLabels;
            if (field.InputElementType is FileuploadInputElementAttribute)
            {
                hide = false;
            }

            WriteLabelStart(hide, field.Id, sb);
            WriteLabelContent(field, sb);
            WriteLabelEnd(sb);
        }

        private static void WritePropertyHeading(FormField field, StringBuilder sb)
        {
            sb.Append("<p class=\"control-label\">");

            WriteLabelContent(field, sb);

            sb.Append("</p>");
        }

        public static void WriteLabelContent(bool isRequired, string label, string link, bool openLinkInNewWindow, StringBuilder sb)
        {
            if (isRequired)
            {
                sb.Append("<span class=\"required\">*</span>");
            }

            if (!String.IsNullOrEmpty(link))
            {
                sb.AppendFormat("<a href=\"{0}\" title=\"{1}\" {2}>{3}</a>",
                    HttpUtility.HtmlAttributeEncode(link),
                    HttpUtility.HtmlAttributeEncode(label),
                    openLinkInNewWindow ? "target=\"_blank\"" : String.Empty,
                    HttpUtility.HtmlEncode(label));
            }
            else
            {
                sb.Append(HttpUtility.HtmlEncode(label));
            }
        }

        private static void WriteLabelContent(FormField field, StringBuilder sb)
        {
            WriteLabelContent(field.IsRequired, field.Label.Label, field.Label.Link, field.Label.OpenLinkInNewWindow, sb);
        }

        public static string WriteErrorClass(string name, ValidationResultList validationResult)
        {
            return validationResult.Any(el => el.AffectedFormIds.Contains(name)) ? RendererImplementation.ErrorClass : String.Empty;
        }

        public static string GetValue(FormField field)
        {
            if (field.Value == null)
            {
                return String.Empty;
            }

            if (field.ValueType == typeof(DateTime))
            {
                return ((DateTime)field.Value).ToString("yyyy-MM-dd");
            }

            if (field.ValueType == typeof(DateTime?))
            {
                var dt = (DateTime?)field.Value;

                return dt.Value.ToString("yyyy-MM-dd");
            }

            return field.Value.ToString();
        }

        public static string GetLocalized(string text)
        {
            return text.Contains("${") ? StringResourceSystemFacade.ParseString(text) : text;
        }

        public static IHtmlString Captcha(FormModel model)
        {
            var requiresCaptchaAttr = model.Attributes.OfType<RequiresCaptchaAttribute>().SingleOrDefault();
            if (requiresCaptchaAttr == null)
            {
                return null;
            }

            var s = requiresCaptchaAttr.Render(model);

            return new HtmlString(s);
        }
    }
}
