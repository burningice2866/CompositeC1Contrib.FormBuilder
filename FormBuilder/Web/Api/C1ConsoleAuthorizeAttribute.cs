using System.Web.Http;
using System.Web.Http.Controllers;

using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.Web.Api
{
    public class C1ConsoleAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return UserValidationFacade.IsLoggedIn();
        }
    }
}