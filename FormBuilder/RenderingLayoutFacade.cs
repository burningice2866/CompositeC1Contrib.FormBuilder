using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Composite.Core.Xml;

namespace CompositeC1Contrib.FormBuilder
{
    public class RenderingLayoutFacade
    {
        private const string RenderingLayoutFileName = "RenderingLayout.xml";

        public static bool HasCustomRenderingLayout(string formName)
        {
            var file = Path.Combine(FormModelsFacade.FormsPath, formName, RenderingLayoutFileName);

            return File.Exists(file);
        }

        public static XhtmlDocument GetRenderingLayout(string formName)
        {
            if (HasCustomRenderingLayout(formName))
            {
                var file = Path.Combine(FormModelsFacade.FormsPath, formName, RenderingLayoutFileName);
                var fileContent = File.ReadAllText(file);

                return XhtmlDocument.Parse(fileContent);
            }

            var doc = new XhtmlDocument();
            var model = FormModelsFacade.GetModels().Single(m => m.Name == formName);

            foreach (var field in model.Fields.Where(f => f.Label != null))
            {
                doc.Body.Add(new XElement(Namespaces.Xhtml + "p", String.Format("%{0}%", field.Name)));
            }

            return doc;
        }

        public static void SaveRenderingLayout(string formName, XhtmlDocument markup)
        {
            var dir = Path.Combine(FormModelsFacade.FormsPath, formName);
            var file = Path.Combine(dir, RenderingLayoutFileName);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (markup.IsEmpty)
            {
                File.Delete(file);
            }
            else
            {
                File.WriteAllText(file, markup.ToString());
            }
        }
    }
}
