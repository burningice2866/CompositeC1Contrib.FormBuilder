using System.Web.Routing;

using CompositeC1Contrib.FormBuilder.Excel.Web.Api.Formatters;
using CompositeC1Contrib.Web;

using Owin;

namespace CompositeC1Contrib.FormBuilder.Excel.Web
{
    public static class OwinExtensions
    {
        public static void UseCompositeC1ContribFormBuilderExcel(this IAppBuilder app)
        {
            var routes = RouteTable.Routes;

            routes.AddGenericHandler<ModelSubmitsExcelHandler>("formbuilder/{name}/submits.xlsx");
        }
    }
}
