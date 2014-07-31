using System.Web;
using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.SessionState;

namespace CompositeC1Contrib.FormBuilder.Web.Api
{
    public class RequireSessionStateControllerHandler : HttpControllerHandler, IRequiresSessionState
    {
        public RequireSessionStateControllerHandler(RouteData routeData) : base(routeData) { }
    }

    public class RequireSessionStateControllerRouteHandler : HttpControllerRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new RequireSessionStateControllerHandler(requestContext.RouteData);
        }
    }
}
