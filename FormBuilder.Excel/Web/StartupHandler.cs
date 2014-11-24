using System.Web.Http;

using Composite.Core.Application;

using CompositeC1Contrib.FormBuilder.Excel.Web.Api.Formatters;

namespace CompositeC1Contrib.FormBuilder.Excel.Web
{
    [ApplicationStartup]
    public class StartupHandler
    {
        public static void OnBeforeInitialize()
        {
            InitWebApi();
        }

        private static void InitWebApi()
        {
            GlobalConfiguration.Configure(config => config.Formatters.Add(new ExcelMediaTypeFormatter()));
        }

        public static void OnInitialized() { }
    }
}
