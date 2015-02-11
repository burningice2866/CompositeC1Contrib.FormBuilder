using System;
using System.Web;

using CompositeC1Contrib.FormBuilder.Configuration;
using CompositeC1Contrib.FormBuilder.Validation;
using CompositeC1Contrib.FormBuilder.Web;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RequiresCaptchaAttribute : Attribute
    {
        private static readonly CaptchaProvider Provider;

        static RequiresCaptchaAttribute()
        {
            var captchaConfig = FormBuilderConfiguration.GetSection().Captcha;

            Provider = (CaptchaProvider)captchaConfig.Providers[captchaConfig.DefaultProvider];
        }

        public string Render(IFormModel model)
        {
            return Provider.Render(model);
        }

        public void Validate(HttpContextBase ctx, ValidationResultList validationMessages)
        {
            Provider.Validate(ctx, validationMessages);
        }
    }
}
