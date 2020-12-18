using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;

using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.Web.Api.Handlers
{
    public abstract class FormBuilderHttpHandlerBase : IHttpHandler
    {
        protected HttpContextBase Context { get; private set; }

        public bool IsReusable { get; } = false;

        public void ProcessRequest(HttpContext context)
        {
            ProcessRequest(new HttpContextWrapper(context));
        }

        public void ProcessRequest(HttpContextBase context)
        {
            Context = context;

            SetAcceptLanguage();
            ProcessRequest();
        }

        private void SetAcceptLanguage()
        {
            var cultureHeader = Context.Request.Headers["X-FormBuilder-Culture"];
            if (!String.IsNullOrEmpty(cultureHeader))
            {
                var ci = new CultureInfo(cultureHeader);

                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = ci;

                return;
            }

            var userLanguage = Context.Request.UserLanguages?.FirstOrDefault();
            if (!String.IsNullOrEmpty(userLanguage))
            {
                var ci = new CultureInfo(userLanguage);

                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = ci;

                return;
            }
        }

        protected abstract void ProcessRequest();

        protected bool EnsureHttpMethod(string method)
        {
            if (!Context.Request.HttpMethod.Equals(method, StringComparison.OrdinalIgnoreCase))
            {
                Context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return false;
            }

            return true;
        }

        protected bool EnsureLoggedIn()
        {
            var isLoggedIn = UserValidationFacade.IsLoggedIn();
            if (!isLoggedIn)
            {
                Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                return false;
            }

            return true;
        }
    }
}
