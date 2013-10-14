using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Validation;
using CompositeC1Contrib.FormBuilder.Dependencies;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public static class FormRenderer
    {
        public static IHtmlString NameFor(BaseForm form, PropertyInfo prop)
        {
            return new HtmlString(prop.Name);
        }

        public static IHtmlString FieldFor(BaseForm form, FormOptions options, PropertyInfo prop)
        {
            var htmlAttributes = new Dictionary<string, object>();

            return FieldFor(form, options, prop, htmlAttributes);
        }

        public static IHtmlString FieldFor(BaseForm form, FormOptions options, PropertyInfo prop, IDictionary<string, object> htmlAttributes)
        {
            return writeRow(form, options, prop, htmlAttributes);
        }

        public static IHtmlString WriteErrors(this BaseForm form)
        {
            var sb = new StringBuilder();
            var validationResult = form.Validate().ToList();

            if (validationResult.Any())
            {
                sb.Append("<div class=\"error_notification\">");

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

        private static IHtmlString writeRow(BaseForm form, FormOptions options, PropertyInfo prop, IDictionary<string, object> htmlAttributes)
        {
            var attributes = prop.GetCustomAttributes(true);

            bool required = false;
            var name = prop.Name;
            FieldLabelAttribute label = null;
            string help = null;
            InputType type = GetDefaultInputType(prop.PropertyType);

            foreach (var attr in attributes)
            {
                if (attr is RequiredFieldAttribute)
                {
                    required = true;
                }

                var labelAttribute = attr as FieldLabelAttribute;
                if (labelAttribute != null)
                {
                    label = labelAttribute;
                }

                var inputTypeAttribute = attr as InputFieldTypeAttribute;
                if (inputTypeAttribute != null)
                {
                    type = inputTypeAttribute.InputType;
                }

                var helpAttribute = attr as FieldHelpAttribute;
                if (helpAttribute != null)
                {
                    help = helpAttribute.Help;
                }
            }

            var sb = new StringBuilder();
            var includeLabel = showLabel(type, prop);
            var validationResult = form.Validate().ToList();
            var fieldId = getFieldId(prop);

            sb.AppendFormat("<div id=\"form-field-{0}\" class=\"control-group control-{1} {2} {3} \"", name, getFieldName(type), WriteErrorClass(name, validationResult), required ? "required" : String.Empty);

            var dependencyAttributes = attributes.Where(f => f is FormDependencyAttribute).Cast<FormDependencyAttribute>().ToList();
            if (dependencyAttributes.Any())
            {
                var dependencyObj = new StringBuilder();

                dependencyObj.Append("[ ");

                for (int i = 0; i < dependencyAttributes.Count; i++)
                {
                    var dependencyAttribute = dependencyAttributes[i];
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

                    if (i < (dependencyAttributes.Count - 1))
                    {
                        dependencyObj.Append(", ");
                    }
                }

                dependencyObj.Append(" ]");

                sb.AppendFormat(" data-dependency=\"{0}\"", dependencyObj);
            }

            sb.Append(">");

            if (includeLabel)
            {
                writeLabel(label, fieldId, name, required, options.HideLabels, sb);
            }
            else
            {
                writePropertyHeading(label, name, required, sb);
            }

            sb.Append("<div class=\"controls\">");

            writeField(type, name, help, label, required, form, options, prop, sb, htmlAttributes);

            sb.Append("</div></div>");

            return new HtmlString(sb.ToString());
        }


        private static bool showLabel(InputType type, PropertyInfo prop)
        {
            if (prop.PropertyType == typeof(bool))
            {
                return true;
            }

            if (type == InputType.Checkbox || type == InputType.RadioButton)
            {
                return false;
            }

            return true;
        }

        private static string getFieldId(PropertyInfo prop)
        {
            return prop.DeclaringType.FullName + prop.Name;
        }

        private static InputType GetDefaultInputType(Type type)
        {
            if (type == typeof(bool)) return InputType.Checkbox;
            if (type == typeof(FormFile)) return InputType.Fileupload;

            return InputType.Textbox;
        }

        private static IEnumerable<RichTextListItem> getOptions(BaseForm form, PropertyInfo prop)
        {
            var datasourceAttribute = prop.GetCustomAttributes(typeof(DataSourceAttribute), true).FirstOrDefault() as DataSourceAttribute;
            if (datasourceAttribute == null)
            {
                return null;
            }

            var ds = datasourceAttribute.GetData(form);
            if (ds == null)
            {
                return null;
            }

            var dict = ds as IDictionary<string, string>;
            if (dict != null)
            {
                return dict.Select(f => new RichTextListItem(f.Key, f.Value));
            }

            if (ds is IEnumerable<string>)
            {
                return (ds as IEnumerable<string>).Select(str => new RichTextListItem(str, str));
            }

            var list = ds as IEnumerable<RichTextListItem>;
            if (list != null)
            {
                return list;
            }

            throw new InvalidOperationException("Unsupported data source type: " + ds.GetType().FullName);
        }

        private static void writeField(InputType type, string name, string help, FieldLabelAttribute attrLabel, bool required, BaseForm form, FormOptions options, PropertyInfo prop, StringBuilder sb, IDictionary<string, object> htmlAttributes)
        {
            var value = prop.GetValue(form, null);
            var strLabel = attrLabel == null ? name : attrLabel.Label;
            var fieldId = getFieldId(prop);

            if (!String.IsNullOrWhiteSpace(help))
            {
                sb.Append("<div class=\"input-append\">");
            }

            switch (type)
            {
                case InputType.Checkbox:

                    if (prop.PropertyType == typeof(bool))
                    {
                        var check = (bool)value ? "checked=\"checked\"" : "";

                        sb.AppendFormat("<input type=\"checkbox\" name=\"{0}\" id=\"{1}\" value=\"on\" title=\"{2}\" {3} {4} />",
                            name,
                            fieldId,
                            strLabel,
                            check,
                            writeClass(htmlAttributes));
                    }
                    else if (prop.PropertyType == typeof(IEnumerable<string>))
                    {
                        var checkboxListOptions = getOptions(form, prop);
                        if (checkboxListOptions != null)
                        {
                            var ix = 0;
                            var list = value == null ? Enumerable.Empty<string>() : (IEnumerable<string>)value;

                            foreach (var item in checkboxListOptions)
                            {
                                sb.Append("<label class=\"checkbox\">");

                                sb.AppendFormat("<input type=\"checkbox\" name=\"{1}\" id=\"{2}\" value=\"{3}\" title=\"{0}\" {4} {5}/> {0} ",
                                    item.StringLabel,
                                    name,
                                    fieldId + "_" + ix++,
                                    item.Key,
                                    writeChecked(list.Contains(item.Key), "checked"),
                                    writeClass(htmlAttributes));

                                sb.Append("</label>");

                                if (item.HtmlLabel != null)
                                {
                                    sb.AppendFormat("<div class=\"label-rich\">{0}</div>", item.HtmlLabel);
                                }
                            }
                        }
                    }

                    break;

                case InputType.RadioButton:

                    var radioButtonListOptions = getOptions(form, prop);
                    if (radioButtonListOptions != null)
                    {
                        var ix = 0;

                        foreach (var item in radioButtonListOptions)
                        {
                            sb.Append("<label class=\"radio\">");

                            sb.AppendFormat("<input type=\"radio\" name=\"{1}\" id=\"{2}\" value=\"{3}\" title=\"{0}\" {4} {5}/> {0}",
                                item.StringLabel,
                                name,
                                fieldId + "_" + ix++,
                                item.Key,
                                (value == null ? "" : writeChecked(isEqual(value, item.Key), "checked")),
                                writeClass(htmlAttributes));

                            sb.Append("</label>");

                            if (item.HtmlLabel != null)
                            {
                                sb.AppendFormat("<div class=\"label-rich\">{0}</div>", item.HtmlLabel);
                            }
                        }
                    }

                    break;

                case InputType.Dropdown:

                    sb.AppendFormat("<select name=\"{0}\" id=\"{1}\" {2}>", name, fieldId, writeClass(htmlAttributes));

                    var dropdownOptions = getOptions(form, prop);
                    if (dropdownOptions != null)
                    {
                        var selectLabel = options.HideLabels ? strLabel : Localization.Widgets_Dropdown_SelectLabel;

                        sb.AppendFormat("<option value=\"\" selected=\"selected\" disabled=\"disabled\">{0}</option>", HttpUtility.HtmlEncode(selectLabel));

                        foreach (var item in dropdownOptions)
                        {
                            sb.AppendFormat("<option value=\"{0}\" {1}>{2}</option>",
                                HttpUtility.HtmlAttributeEncode(item.Key),
                                writeChecked(item.Key == (value ?? String.Empty).ToString(), "selected"),
                                HttpUtility.HtmlEncode(item.StringLabel));
                        }
                    }

                    sb.Append("</select>");

                    break;

                case InputType.TextArea:
                    var textarea = "<textarea name=\"{0}\" id=\"{1}\" rows=\"5\" cols=\"40\" title=\"{2}\" placeholder=\"{2}\" {3}>{4}</textarea>";

                    sb.AppendFormat(textarea,
                        name,
                        fieldId,
                        strLabel,
                        writeClass(htmlAttributes),
                        HttpUtility.HtmlEncode(value));

                    break;

                case InputType.Textbox:
                case InputType.Password:

                    var s = "<input type=\"{0}\" name=\"{1}\" id=\"{2}\" value=\"{3}\" title=\"{4}\" placeholder=\"{4}\" {5} />";

                    sb.AppendFormat(s,
                        type == InputType.Textbox ? evaluateTextboxType(prop) : "password",
                        name,
                        fieldId,
                        HttpUtility.HtmlEncode(value),
                        strLabel,
                        writeClass(htmlAttributes));

                    break;

                case InputType.Fileupload:

                    sb.AppendFormat("<input type=\"file\" name=\"{0}\" id=\"{1}\" />", name, fieldId);

                    break;
            }

            if (!String.IsNullOrWhiteSpace(help))
            {
                sb.Append("<div class=\"info-block\">");
                sb.Append("<span class=\"add-on info-icon\">i</span>");
                sb.AppendFormat("<div class=\"info-msg\">{0}</div>", HttpUtility.HtmlEncode(help));
                sb.Append("</div>");
                sb.Append("</div>");
            }
        }

        private static string evaluateTextboxType(PropertyInfo prop)
        {
            var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            if (type == typeof(DateTime))
            {
                return "date";
            }

            if (type == typeof(int))
            {
                return "number";
            }

            if (type == typeof(string))
            {
                var attributes = prop.GetCustomAttributes(true);
                if (attributes.Any(f => f is EmailExistsValidatorAttribute))
                {
                    return "email";
                }
            }

            return "text";
        }

        private static string getFieldName(InputType type)
        {
            switch (type)
            {
                case InputType.Checkbox: return "checkbox";
                case InputType.Dropdown: return "selectbox";
                case InputType.TextArea: return "textarea";
                case InputType.RadioButton: return "radio";

                case InputType.Password:
                case InputType.Textbox: return "textbox";

                default: return "textbox";
            }
        }

        private static string writeClass(IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes.ContainsKey("class"))
            {
                return "class=\"" + htmlAttributes["class"] + "\"";
            }

            return String.Empty;
        }

        private static string writeChecked(bool write, string attr)
        {
            if (write)
            {
                return String.Format("{0}=\"{0}\"", attr);
            }

            return String.Empty;
        }

        private static bool isEqual(object obj, string value)
        {
            if (obj is bool)
            {
                return bool.Parse(value) == (bool)obj;
            }

            return obj.ToString() == value;
        }

        private static string writeLabel(FieldLabelAttribute label, string fieldId, string name, bool required, bool hide, StringBuilder sb)
        {
            sb.AppendFormat("<label class=\"control-label {0}\" for=\"{1}\">", hide ? "hide-text " : String.Empty, fieldId);

            writeLabelContent(required, label, name, sb);

            sb.Append(":");
            sb.Append("</label>");

            return sb.ToString();
        }

        private static string writePropertyHeading(FieldLabelAttribute label, string name, bool required, StringBuilder sb)
        {
            sb.Append("<p class=\"control-label\">");

            writeLabelContent(required, label, name, sb);

            sb.Append("</p>");

            return sb.ToString();
        }

        private static void writeLabelContent(bool required, FieldLabelAttribute label, string name, StringBuilder sb)
        {
            var title = label == null ? name : label.Label;

            if (required)
            {
                sb.Append("<span class=\"required\">*</span>");
            }

            if (label != null && !String.IsNullOrEmpty(label.Link))
            {
                sb.AppendFormat("<a href=\"{0}\" title=\"{1}\" {2}>{1}</a>", label.Link, title, label.OpenLinkInNewWindow ? "target=\"_blank\"" : String.Empty);
            }
            else
            {
                sb.Append(title);
            }
        }

        public static string WriteErrorClass(string name, IEnumerable<FormValidationRule> validationResult)
        {
            if (validationResult.Any(el => el.AffectedFormIds.Contains(name)))
            {
                return "error";
            }

            return String.Empty;
        }
    }
}
