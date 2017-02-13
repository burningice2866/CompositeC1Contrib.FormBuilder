using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Data;
using CompositeC1Contrib.FormBuilder.Web.Api.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CompositeC1Contrib.FormBuilder.Web.UI.Rendering
{
    public abstract class FormRenderer
    {
        public bool ShowStarOnRequiredFields { get; set; }
        public bool HideLabels { get; set; }
        public bool Horizontal { get; set; }
        public int LabelWidth { get; set; }

        public abstract string ValidationSummaryClass { get; }
        public abstract string ErrorClass { get; }
        public abstract string ParentGroupClass { get; }
        public abstract string FieldGroupClass { get; }
        public abstract string HideLabelClass { get; }
        public abstract string FormLabelClass { get; }
        public abstract string FormControlClass { get; }

        public FormRenderer()
        {
            ShowStarOnRequiredFields = false;
            HideLabels = false;
            Horizontal = false;
            LabelWidth = 2;
        }

        public abstract string FormControlLabelClass(InputElementTypeAttribute inputElement);

        public IHtmlString ValidationSummary<T>(BaseFormBuilderRequestContext<T> context) where T : IModelInstance
        {
            var sb = new StringBuilder();

            sb.Append("<div class=\"" + ValidationSummaryClass + "\">");

            if (context.IsOwnSubmit && context.ValidationResult.Any())
            {
                var s = ValidationSummary(context.ValidationResult.Select(r => new ValidationError
                {
                    AffectedFields = r.AffectedFormIds,
                    Message = r.ValidationMessage
                }));

                sb.Append(s);
            }

            sb.Append("</div>");

            return new HtmlString(sb.ToString());
        }

        public virtual string ValidationSummary(IEnumerable<ValidationError> errors)
        {
            var fieldsWritten = new List<string>();
            var sb = new StringBuilder();

            if (!String.IsNullOrEmpty(Localization.T("Validation.ErrorNotification.Header")))
            {
                sb.Append("<p>" + HttpUtility.HtmlEncode(Localization.T("Validation.ErrorNotification.Header")) + "</p>");
            }

            sb.Append("<ul>");

            foreach (var itm in errors)
            {
                if (itm.AffectedFields.Any(fieldsWritten.Contains))
                {
                    continue;
                }

                sb.Append("<li>" + itm.Message + "</li>");

                fieldsWritten.AddRange(itm.AffectedFields);
            }

            sb.Append("</ul>");

            if (!String.IsNullOrEmpty(Localization.T("Validation.ErrorNotification.Footer")))
            {
                sb.Append("<p>" + HttpUtility.HtmlEncode(Localization.T("Validation.ErrorNotification.Footer")) + "</p>");
            }

            return sb.ToString();
        }

        public IHtmlString FieldFor(BaseFormBuilderRequestContext context, FormField field)
        {
            return FieldFor(context, field, new Dictionary<string, string>());
        }

        public IHtmlString FieldFor(BaseFormBuilderRequestContext context, FormField field, Dictionary<string, string> htmlAttributes)
        {
            return WriteRow(context, field, htmlAttributes);
        }

        public IHtmlString InputFor(BaseFormBuilderRequestContext context, FormField field, Dictionary<string, string> htmlAttributes)
        {
            var sb = new StringBuilder();

            WriteField(context, field, sb, htmlAttributes);

            return new HtmlString(sb.ToString());
        }

        public IHtmlString NameFor(FormField field)
        {
            return new HtmlString(field.Name);
        }

        private IHtmlString WriteRow(BaseFormBuilderRequestContext context, FormField field, IDictionary<string, string> htmlAttributes)
        {
            var sb = new StringBuilder();

            WriteRowStart(field.Name, field.InputElementType.ElementName,
                WriteErrorClass(field.Name, context), field.IsRequired,
                builder => DependencyAttributeFor(field, builder), sb);

            if (!HideLabels)
            {
                if (!(field.InputElementType is CheckboxInputElementAttribute && field.ValueType == typeof(bool)))
                {
                    var showLabel = ShowLabel(field);
                    if (showLabel)
                    {
                        WriteLabel(field, sb);
                    }
                    else
                    {
                        WritePropertyHeading(field, sb);
                    }
                }
            }

            using (new ControlsGroup(sb, this))
            {
                if (!HideLabels)
                {
                    if (field.InputElementType is CheckboxInputElementAttribute && field.ValueType == typeof(bool))
                    {
                        sb.AppendFormat("<div class=\"{0}\">", field.InputElementType.ElementName);
                        sb.Append("   <label>");
                    }
                }

                WriteField(context, field, sb, htmlAttributes);

                if (!HideLabels)
                {
                    if (field.InputElementType is CheckboxInputElementAttribute && field.ValueType == typeof(bool))
                    {
                        WriteLabelContent(field, sb);

                        sb.Append("  </label>");
                        sb.Append("</div>");
                    }
                }
            }

            WriteRowEnd(sb);

            return new HtmlString(sb.ToString());
        }

        public void WriteRowStart(string name, string elementName, string errorClass, bool isRequired, Action<TagBuilder> extraAttributesRenderer, StringBuilder sb)
        {
            if (FieldsRow.Current != null)
            {
                return;
            }

            var tagBuilder = new TagBuilder("div");

            tagBuilder.GenerateId("form-field-" + name);

            tagBuilder.AddCssClass(ParentGroupClass + "-group");
            tagBuilder.AddCssClass("control-" + elementName);
            tagBuilder.AddCssClass(errorClass);

            if (isRequired)
            {
                tagBuilder.AddCssClass("required");
            }

            if (extraAttributesRenderer != null)
            {
                extraAttributesRenderer(tagBuilder);
            }

            sb.Append(tagBuilder.ToString(TagRenderMode.StartTag));
        }

        public void WriteRowEnd(StringBuilder sb)
        {
            if (FieldsRow.Current != null)
            {
                return;
            }

            sb.Append("</div>");
        }

        private bool ShowLabel(FormField field)
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

        public void WriteFieldHelpStart(StringBuilder sb)
        {
            sb.Append("<div class=\"input-append\">");
        }

        public void WriteFieldHelpEnd(string help, StringBuilder sb)
        {
            sb.Append("<div class=\"info-block\">");
            sb.Append("<span class=\"add-on info-icon\">i</span>");
            sb.AppendFormat("<div class=\"info-msg\">{0}</div>", help);
            sb.Append("</div>");
            sb.Append("</div>");
        }

        private void WriteField(BaseFormBuilderRequestContext context, FormField field, StringBuilder sb, IDictionary<string, string> htmlAttributes)
        {
            var str = field.InputElementType.GetHtmlString(context, field, htmlAttributes);

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

        public void DependencyAttributeFor(FormField field, TagBuilder tagBuilder)
        {
            if (!field.DependencyAttributes.Any())
            {
                return;
            }

            var dependencies = field.DependencyAttributes.Select(d => new DependencyModel { Field = d.ReadFromFieldName, Value = d.ResolveRequiredFieldValues() });
            var json = JsonConvert.SerializeObject(dependencies, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            tagBuilder.MergeAttribute("data-dependency", json);

            if (!field.IsDependencyMetRecursive())
            {
                tagBuilder.AddCssClass("hide");
            }
        }

        public string WriteChecked(bool write, string attr)
        {
            return write ? String.Format("{0}=\"{0}\"", attr) : String.Empty;
        }

        public void WriteLabelStart(bool hide, string id, StringBuilder sb)
        {
            var tagBuilder = new TagBuilder("label");

            tagBuilder.AddCssClass("control-label");

            if (Horizontal)
            {
                tagBuilder.AddCssClass("col-sm-" + LabelWidth);
            }

            if (hide)
            {
                tagBuilder.AddCssClass(HideLabelClass);
            }

            tagBuilder.MergeAttribute("for", id);

            sb.AppendFormat(tagBuilder.ToString(TagRenderMode.StartTag));
        }

        public void WriteLabelEnd(StringBuilder sb)
        {
            sb.Append("</label>");
        }

        private void WriteLabel(FormField field, StringBuilder sb)
        {
            var hide = HideLabels;

            if (field.InputElementType is FileuploadInputElementAttribute)
            {
                hide = false;
            }

            WriteLabelStart(hide, field.Id, sb);
            WriteLabelContent(field, sb);
            WriteLabelEnd(sb);
        }

        private void WritePropertyHeading(FormField field, StringBuilder sb)
        {
            sb.Append("<p class=\"control-label\">");

            WriteLabelContent(field, sb);

            sb.Append("</p>");
        }

        public void WriteLabelContent(bool isRequired, string label, StringBuilder sb)
        {
            if (isRequired && ShowStarOnRequiredFields)
            {
                sb.Append("<span class=\"required\">*</span>");
            }

            sb.Append(HttpUtility.HtmlEncode(label));
        }

        private void WriteLabelContent(FormField field, StringBuilder sb)
        {
            WriteLabelContent(field.IsRequired, field.Label, sb);
        }

        public string WriteErrorClass(string name, BaseFormBuilderRequestContext context)
        {
            return context.ValidationResult.Any(el => el.AffectedFormIds.Contains(name)) ? ErrorClass : String.Empty;
        }

        public static IHtmlString Captcha<T>(BaseFormBuilderRequestContext<T> context) where T : class, IModelInstance
        {
            if (!context.ModelInstance.RequiresCaptcha)
            {
                return null;
            }

            var requiresCaptchaAttr = new RequiresCaptchaAttribute();
            var s = requiresCaptchaAttr.Render(context);

            return new HtmlString(s);
        }
    }
}
