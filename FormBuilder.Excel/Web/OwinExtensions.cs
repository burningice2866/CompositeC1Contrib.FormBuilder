using System.Web.Http;

using CompositeC1Contrib.FormBuilder.Excel.Web.Api.Formatters;

using Owin;

namespace CompositeC1Contrib.FormBuilder.Excel.Web
{
    public static class OwinExtensions
    {
        public static void UseCompositeC1ContribFormBuilderExcel(this IAppBuilder app, HttpConfiguration config)
        {
            config.Formatters.Add(new ExcelMediaTypeFormatter());
        }
    }
}
