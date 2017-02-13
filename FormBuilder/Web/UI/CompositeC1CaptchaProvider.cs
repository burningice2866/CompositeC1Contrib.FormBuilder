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
                validationMessages.Add(InputFieldName, Localization.T("Captcha.CompositeC1.Error"));
            }
        }

        public override string Render(BaseFormBuilderRequestContext context)
        {
            var sb = new StringBuilder();
            var encryptedValue = Captcha.CreateEncryptedValue();
            var renderer = context.FormRenderer;

            sb.AppendFormat("<input type=\"hidden\" name=\"{0}\" id=\"{1}\" value=\"{2}\" />",
                HttpUtility.HtmlAttributeEncode(HiddenFieldName),
                HttpUtility.HtmlAttributeEncode(HiddenFieldName),
                HttpUtility.HtmlAttributeEncode(encryptedValue));

            renderer.WriteRowStart(InputFieldName, "captcha", renderer.WriteErrorClass(InputFieldName, context), true, null, sb);

            renderer.WriteLabelStart(false, InputFieldName, sb);
            renderer.WriteLabelContent(true, Localization.T("Captcha.CompositeC1.Label"), sb);
            renderer.WriteLabelEnd(sb);

            using (new ControlsGroup(sb, context.FormRenderer))
            {
                if (!String.IsNullOrEmpty(Localization.T("Captcha.CompositeC1.Help")))
                {
                    renderer.WriteFieldHelpStart(sb);
                }

                sb.AppendFormat("<div class=\"captcha-input\">");
                sb.AppendFormat("<input name=\"{0}\" id=\"{0}\" type=\"text\"  />", InputFieldName);
                sb.AppendFormat("</div>");

                sb.AppendFormat("<div class=\"captcha-img\">");
                sb.AppendFormat("<img src=\"{0}\" />", Captcha.GetImageUrl(encryptedValue));
                sb.AppendFormat("</div>");

                if (!String.IsNullOrEmpty(Localization.T("Captcha.CompositeC1.Help")))
                {
                    renderer.WriteFieldHelpEnd(Localization.T("Captcha.CompositeC1.Help"), sb);
                }
            }

            renderer.WriteRowEnd(sb);

            return sb.ToString();
        }
    }
}
