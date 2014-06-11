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

            if (!Directory.Exists(FormModelsFacade.RootPath))
            {
                Directory.CreateDirectory(FormModelsFacade.RootPath);
            }

            MoveRenderingLayoutToFormsFolder(FormModelsFacade.RootPath);
            MoveSubfoldersToRoot(FormModelsFacade.RootPath);
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

        private static void MoveSubfoldersToRoot(string rootFolder)
        {
            var formsFolder = Path.Combine(rootFolder, "Forms");
            var wizardsFolder = Path.Combine(rootFolder, "Wizards");

            MoveSubfoldersToRoot(rootFolder, formsFolder);
            MoveSubfoldersToRoot(rootFolder, wizardsFolder);
        }

        private static void MoveSubfoldersToRoot(string rootFolder, string subFolder)
        {
            foreach (var directory in Directory.GetDirectories(subFolder))
            {
                var name = new DirectoryInfo(directory).Name;
                if (!name.Contains("."))
                {
                    continue;
                }

                Directory.Move(directory, Path.Combine(rootFolder, name));
            }
        }

        public static void OnInitialized() { }
    }
}
