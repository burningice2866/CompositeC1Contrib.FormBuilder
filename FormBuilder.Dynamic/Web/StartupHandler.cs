using System.IO;
using Composite.Core.Application;

namespace CompositeC1Contrib.FormBuilder.Dynamic.Web
{
    [ApplicationStartup]
    public class StartupHandler
    {
        public static void OnBeforeInitialize()
        {
            var baseFolder = FormModelsFacade.FormsPath;
            var definitionsFolder = Path.Combine(baseFolder, "FormDefinitions");

            if (!Directory.Exists(definitionsFolder))
            {
                return;
            }

            foreach (var file in Directory.GetFiles(definitionsFolder, "*.xml"))
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var folder = Path.Combine(baseFolder, fileName);

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var newFilePath = Path.Combine(folder, "DynamicDefinition.xml");
                File.Move(file, newFilePath);
            }

            Directory.Delete(definitionsFolder);
        }

        public static void OnInitialized()
        {

        }
    }
}
