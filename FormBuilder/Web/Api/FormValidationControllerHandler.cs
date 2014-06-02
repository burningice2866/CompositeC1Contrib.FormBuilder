using System.Web;
using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.SessionState;

namespace CompositeC1Contrib.FormBuilder.Web.Api
{
    public class FormValidationControllerHandler : HttpControllerHandler, IRequiresSessionState
    {
        public FormValidationControllerHandler(RouteData routeData) : base(routeData) { }
    }

    public class FormValidationControllerRouteHandler : HttpControllerRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new FormValidationControllerHandler(requestContext.RouteData);
        }
    }
}
