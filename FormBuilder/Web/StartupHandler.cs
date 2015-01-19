using System.IO;

using Composite.Core.Application;

namespace CompositeC1Contrib.FormBuilder.Web
{
    [ApplicationStartup]
    public class StartupHandler
    {
        public static void OnBeforeInitialize()
        {
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
                if (fileName == null)
                {
                    continue;
                }

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
            if (Directory.Exists(formsFolder))
            {
                MoveSubfoldersToRoot(rootFolder, formsFolder);
            }

            var wizardsFolder = Path.Combine(rootFolder, "Wizards");
            if (Directory.Exists(wizardsFolder))
            {
                MoveSubfoldersToRoot(rootFolder, wizardsFolder);
            }
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
