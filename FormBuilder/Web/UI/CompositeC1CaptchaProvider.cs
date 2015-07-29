using System;
using System.Text;
using System.Web;

using Composite.Core.WebClient.Captcha;

using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class CompositeC1CaptchaProvider : CaptchaProvider
    {
        private const string HiddenFieldName = "__captchaEncrypted";
        private const string InputFieldName = "__captchaValue";

        public override void Validate(HttpContextBase ctx, ValidationResultList validationMessages)
        {
            string value;
            DateTime dt;

            var encryptedValue = ctx.Request.Form[HiddenFieldName];
            var postedValue = ctx.Request.Form[InputFieldName];

            var isValid = Captcha.Decrypt(encryptedValue, out dt, out value) && value == postedValue;
            if (!isValid)
            {
                validationMessages.Add(InputFieldName, Localization.Captcha_CompositeC1_Error);
            }
        }

        public override string Render(BaseFormBuilderRequestContext context)
        {
            var sb = new StringBuilder();
            var encryptedValue = Captcha.CreateEncryptedValue();

            sb.AppendFormat("<input type=\"hidden\" name=\"{0}\" id=\"{1}\" value=\"{2}\" />",
                HttpUtility.HtmlAttributeEncode(HiddenFieldName),
                HttpUtility.HtmlAttributeEncode(HiddenFieldName),
                HttpUtility.HtmlAttributeEncode(encryptedValue));

            FormRenderer.WriteRowStart(InputFieldName, "captcha", FormRenderer.WriteErrorClass(InputFieldName, context), true, null, sb, context.Options);

            FormRenderer.WriteLabelStart(false, InputFieldName, context.Options, sb);
            FormRenderer.WriteLabelContent(true, Localization.Captcha_CompositeC1_Label, String.Empty, false, sb);
            FormRenderer.WriteLabelEnd(sb);

            using (new ControlsGroup(sb, context.Options))
            {
                if (!String.IsNullOrEmpty(Localization.Captcha_CompositeC1_Help))
                {
                    FormRenderer.WriteFieldHelpStart(sb);
                }

                sb.AppendFormat("<div class=\"captcha-input\">");
                sb.AppendFormat("<input name=\"{0}\" id=\"{0}\" type=\"text\"  />", InputFieldName);
                sb.AppendFormat("</div>");

                sb.AppendFormat("<div class=\"captcha-img\">");
                sb.AppendFormat("<img src=\"{0}\" />", Captcha.GetImageUrl(encryptedValue));
                sb.AppendFormat("</div>");

                if (!String.IsNullOrEmpty(Localization.Captcha_CompositeC1_Help))
                {
                    FormRenderer.WriteFieldHelpEnd(Localization.Captcha_CompositeC1_Help, sb);
                }
            }

            FormRenderer.WriteRowEnd(sb);

            return sb.ToString();
        }
    }
}
