using System.Configuration.Provider;
using System.Web;

using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder.Web
{
    public abstract class CaptchaProvider : ProviderBase
    {
        public abstract string Render(IFormModel model);
        public abstract void Validate(HttpContextBase ctx, ValidationResultList validationMessages);
    }
}
