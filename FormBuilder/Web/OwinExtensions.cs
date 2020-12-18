using System.IO;
using System.Web.Routing;

using Composite.Core.IO;

using CompositeC1Contrib.FormBuilder.Web.Api.Handlers;
using CompositeC1Contrib.Web;

using Owin;

namespace CompositeC1Contrib.FormBuilder.Web
{
    public static class OwinExtensions
    {
        public static void UseCompositeC1ContribFormBuilder(this IAppBuilder app)
        {
            var routes = RouteTable.Routes;

            routes.AddGenericHandler<ValidationHandler>("formbuilder/validation");
            routes.AddGenericHandler<RendererHandler>("formbuilder/renderer/validationsummary");
            routes.AddGenericHandler<ModelSubmitsCsvHandler>("formbuilder/{name}/submits.csv");

            Init();
        }

        private static void Init()
        {
            if (!C1Directory.Exists(ModelsFacade.RootPath))
            {
                C1Directory.CreateDirectory(ModelsFacade.RootPath);
            }

            MoveRenderingLayoutToFormsFolder(ModelsFacade.RootPath);
            MoveSubfoldersToRoot(ModelsFacade.RootPath);
        }

        private static void MoveRenderingLayoutToFormsFolder(string baseFolder)
        {
            var layoutsFolder = Path.Combine(baseFolder, "FormRenderingLayouts");
            if (!C1Directory.Exists(layoutsFolder))
            {
                return;
            }

            foreach (var file in C1Directory.GetFiles(layoutsFolder, "*.xml"))
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                if (fileName == null)
                {
                    continue;
                }

                var folder = Path.Combine(baseFolder, fileName);
                if (!C1Directory.Exists(folder))
                {
                    C1Directory.CreateDirectory(folder);
                }

                var newFilePath = Path.Combine(folder, "RenderingLayout.xml");

                File.Move(file, newFilePath);
            }

            C1Directory.Delete(layoutsFolder);
        }

        private static void MoveSubfoldersToRoot(string rootFolder)
        {
            var formsFolder = Path.Combine(rootFolder, "Forms");
            if (C1Directory.Exists(formsFolder))
            {
                MoveSubfoldersToRoot(rootFolder, formsFolder);
            }

            var wizardsFolder = Path.Combine(rootFolder, "Wizards");
            if (C1Directory.Exists(wizardsFolder))
            {
                MoveSubfoldersToRoot(rootFolder, wizardsFolder);
            }
        }

        private static void MoveSubfoldersToRoot(string rootFolder, string subFolder)
        {
            foreach (var directory in C1Directory.GetDirectories(subFolder))
            {
                var name = new C1DirectoryInfo(directory).Name;
                if (!name.Contains("."))
                {
                    continue;
                }

                C1Directory.Move(directory, Path.Combine(rootFolder, name));
            }
        }
    }
}
