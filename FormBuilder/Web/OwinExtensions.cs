using System.Web.Http;
using System.Web.Routing;

using CompositeC1Contrib.FormBuilder.Web.Api;
using CompositeC1Contrib.FormBuilder.Web.Api.Formatters;

using Owin;

namespace CompositeC1Contrib.FormBuilder.Web
{
    public static class OwinExtensions
    {
        public static void UseCompositeC1ContribFormBuilder(this IAppBuilder app, HttpConfiguration config)
        {
            var routes = RouteTable.Routes;

            routes.MapHttpRoute("Validation", "formbuilder/validation", new { controller = "validation" }).RouteHandler = new RequireSessionStateControllerRouteHandler();
            routes.MapHttpRoute("Submit", "formbuilder/{name}/submits.{ext}", new { controller = "formsubmits" }).RouteHandler = new RequireSessionStateControllerRouteHandler();

            config.Formatters.Add(new CsvMediaTypeFormatter());
        }
    }
}
