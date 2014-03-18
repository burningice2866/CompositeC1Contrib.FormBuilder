using System.IO;
using System.Web.Http;

using Composite.Core.Application;

namespace CompositeC1Contrib.FormBuilder.Web
{
    [ApplicationStartup]
    public class StartupHandler
    {
        public static void OnBeforeInitialize()
        {
            var config = GlobalConfiguration.Configuration;

            config.Routes.MapHttpRoute("Form validator", "formbuilder/{controller}");

            var baseFolder = FormModelsFacade.FormsPath;
            var layoutsFolder = Path.Combine(baseFolder, "FormRenderingLayouts");

            if (!Directory.Exists(layoutsFolder))
            {
                return;
            }

            foreach (var file in Directory.GetFiles(layoutsFolder, "*.xml"))
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var folder = Path.Combine(baseFolder, fileName);

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var newFilePath = Path.Combine(folder, "RenderingLayout.xml");
                File.Move(file, newFilePath);
            }

            Directory.Delete(layoutsFolder);
        }

        public static void OnInitialized() { }
    }
}
