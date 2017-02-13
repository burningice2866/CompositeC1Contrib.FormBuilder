using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;

using Newtonsoft.Json;

using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class GoogleReCAPTCHAProvider : CaptchaProvider
    {
        public class Response
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("error-codes")]
            public string[] ErrorCodes { get; set; }
        }

        private const string SiteVerifyUrl = "https://www.google.com/recaptcha/api/siteverify";
        private const string HiddenFieldName = "g-recaptcha-response";
        private const string InputFieldName = "__captchaValue";

        private static readonly string Version = typeof(GoogleReCAPTCHAProvider).Name;

        private string _siteKey;
        private string _secret;

        public override void Initialize(string name, NameValueCollection config)
        {
            _siteKey = config["siteKey"];
            if (String.IsNullOrEmpty(_siteKey))
            {
                throw new ProviderException("SiteKey needs to be set");
            }

            config.Remove("siteKey");

            _secret = config["secret"];
            if (String.IsNullOrEmpty(_siteKey))
            {
                throw new ProviderException("Secret needs to be set");
            }

            config.Remove("secret");

            base.Initialize(name, config);
        }

        public override string Render(BaseFormBuilderRequestContext context)
        {
            var sb = new StringBuilder();
            var renderer = context.FormRenderer;

            renderer.WriteRowStart(InputFieldName, "captcha", renderer.WriteErrorClass(InputFieldName, context), true, null, sb);

            renderer.WriteLabelStart(false, InputFieldName, sb);
            renderer.WriteLabelContent(true, Localization.T("Captcha.GoogleReCAPTCHA.Label"), sb);
            renderer.WriteLabelEnd(sb);

            using (new ControlsGroup(sb, renderer))
            {
                if (!String.IsNullOrEmpty(Localization.T("Captcha.GoogleReCAPTCHA.Help")))
                {
                    renderer.WriteFieldHelpStart(sb);
                }

                sb.AppendFormat("<div class=\"g-recaptcha captcha-img\" data-sitekey=\"{0}\"></div>", _siteKey);
                sb.AppendFormat("<script type=\"text/javascript\" src=\"https://www.google.com/recaptcha/api.js?hl={0}\"></script>", Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);

                if (!String.IsNullOrEmpty(Localization.T("Captcha.CompositeC1.Help")))
                {
                    renderer.WriteFieldHelpEnd(Localization.T("Captcha.GoogleReCAPTCHA.Help"), sb);
                }
            }

            renderer.WriteRowEnd(sb);

            return sb.ToString();
        }

        public override void Validate(HttpContextBase ctx, ValidationResultList validationMessages)
        {
            var response = ctx.Request.Form[HiddenFieldName];
            var ip = ctx.Request.UserHostAddress;
            var url = String.Format("{0}?secret={1}&remoteip={2}&v={3}&response={4}", SiteVerifyUrl, _secret, ip, Version, response);

            using (var client = new HttpClient())
            {
                var resultString = client.GetStringAsync(url).Result;
                var result = JsonConvert.DeserializeObject<Response>(resultString);

                if (!result.Success)
                {
                    validationMessages.Add(InputFieldName, String.Join(", ", result.ErrorCodes));
                }
            }
        }
    }
}
