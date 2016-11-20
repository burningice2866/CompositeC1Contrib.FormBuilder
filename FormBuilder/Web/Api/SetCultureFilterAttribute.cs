using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CompositeC1Contrib.FormBuilder.Web.Api
{
    public class SetCultureFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var cultureHeader = actionContext.ControllerContext.Request.Headers.SingleOrDefault(h => h.Key == "X-FormBuilder-Culture");
            if (!String.IsNullOrEmpty(cultureHeader.Key))
            {
                var ci = new CultureInfo(cultureHeader.Value.Single());

                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = ci;
            }

            base.OnActionExecuting(actionContext);
        }
    }
}
