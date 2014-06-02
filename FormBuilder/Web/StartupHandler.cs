using System.IO;
using System.Web.Http;
using System.Web.Routing;

using Composite.Core.Application;

using CompositeC1Contrib.FormBuilder.Web.Api;

namespace CompositeC1Contrib.FormBuilder.Web
{
    [ApplicationStartup]
    public class StartupHandler
    {
        public static void OnBeforeInitialize()
        {
            RouteTable.Routes.MapHttpRoute("Form validator", "Form validator", new { controller = "attachment" }).RouteHandler = new FormValidationControllerRouteHandler();

            var formsFolder = Path.Combine(FormModelsFacade.RootPath, "Forms");
            if (!Directory.Exists(formsFolder))
            {
                Directory.CreateDirectory(formsFolder);
            }

            MoveRenderingLayoutToFormsFolder(FormModelsFacade.RootPath);
            MoveFormDefinitionFoldersToExplcitFormSubFolder(FormModelsFacade.RootPath, formsFolder);
        }

        private static void MoveRenderingLayoutToFormsFolder(string baseFolder)
        {
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

        private static void MoveFormDefinitionFoldersToExplcitFormSubFolder(string baseFolder, string formsFolder)
        {
            foreach (var directory in Directory.GetDirectories(baseFolder))
            {
                var name = new DirectoryInfo(directory).Name;
                if (!name.Contains("."))
                {
                    continue;
                }

                Directory.Move(directory, Path.Combine(formsFolder, name));
            }
        }

        public static void OnInitialized() { }
    }
}
