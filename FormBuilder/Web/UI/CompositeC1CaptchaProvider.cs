using System;
using System.Text;
using System.Web;

using Composite.Core.WebClient.Captcha;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class CompositeC1CaptchaProvider : CaptchaProvider
    {
        private readonly Lazy<string> _encryptedValue = new Lazy<string>(Captcha.CreateEncryptedValue);

        private const string HiddenFieldName = "__captchaEncrypted";

        public string EncryptedValue
        {
            get { return _encryptedValue.Value; }
        }

        public override void Validate(HttpContextBase ctx, ValidationResultList validationMessages)
        {
            string value;
            DateTime dt;

            var isValid = Captcha.Decrypt(EncryptedValue, out dt, out value) && value == ctx.Request.Form[HiddenFieldName];
            if (!isValid)
            {
                validationMessages.Add(RequiresCaptchaAttribute.InputName, Localization.Captcha_Error);
            }
        }

        public override string Render(IFormModel model)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("<input type=\"hidden\" name=\"{0}\" id=\"{1}\" value=\"{2}\" />",
                HttpUtility.HtmlAttributeEncode(HiddenFieldName),
                HttpUtility.HtmlAttributeEncode(HiddenFieldName),
                HttpUtility.HtmlAttributeEncode(EncryptedValue));

            FormRenderer.WriteRowStart(RequiresCaptchaAttribute.InputName, "captcha", FormRenderer.WriteErrorClass(RequiresCaptchaAttribute.InputName, model.ValidationResult), true, null, sb);

            FormRenderer.WriteLabelStart(false, RequiresCaptchaAttribute.InputName, sb);
            FormRenderer.WriteLabelContent(true, Localization.Captcha_Label, String.Empty, false, sb);
            FormRenderer.WriteLabelEnd(sb);

            using (new ControlsGroup(sb))
            {
                if (!String.IsNullOrEmpty(Localization.Captcha_Help))
                {
                    FormRenderer.WriteFieldHelpStart(sb);
                }

                sb.AppendFormat("<div class=\"captcha-input\">");
                sb.AppendFormat("<input name=\"{0}\" id=\"{0}\" type=\"text\"  />", RequiresCaptchaAttribute.InputName);
                sb.AppendFormat("</div>");

                sb.AppendFormat("<div class=\"captcha-img\">");
                sb.AppendFormat("<img src=\"/Renderers/Captcha.ashx?value={0}\" />", EncryptedValue);
                sb.AppendFormat("</div>");


                if (!String.IsNullOrEmpty(Localization.Captcha_Help))
                {
                    FormRenderer.WriteFieldHelpEnd(Localization.Captcha_Help, sb);
                }
            }

            FormRenderer.WriteRowEnd(sb);

            return sb.ToString();
        }
    }
}
