using System.Configuration.Provider;
using System.Web;

using CompositeC1Contrib.FormBuilder.Validation;
using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder.Web
{
    public abstract class CaptchaProvider : ProviderBase
    {
        public abstract string Render(ValidationResultList validationResult, FormOptions options);
        public abstract void Validate(HttpContextBase ctx, ValidationResultList validationMessages);
    }
}
