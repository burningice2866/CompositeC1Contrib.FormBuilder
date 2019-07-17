using System;
using System.Linq;
using System.Xml.Linq;

using Composite.Core.Xml;

namespace CompositeC1Contrib.FormBuilder
{
    public class RenderingLayoutFacade
    {
        public static bool HasCustomRenderingLayout(string formName)
        {
            var key = GetKey(formName);

            var layout = Localization.T(key);
            if (layout == null)
            {
                return false;
            }

            var xhtml = XhtmlDocument.Parse(layout);

            return !xhtml.IsEmpty;
        }

        public static XhtmlDocument GetRenderingLayout(string formName)
        {
            if (HasCustomRenderingLayout(formName))
            {
                var key = GetKey(formName);
                var layout = Localization.T(key);

                return XhtmlDocument.Parse(layout);
            }

            var model = ModelsFacade.GetModel(formName);
            if (model == null)
            {
                throw new ArgumentException($"Form '{formName}' not loaded");
            }

            var doc = new XhtmlDocument();

            foreach (var field in model.Fields.Where(f => f.Label != null))
            {
                doc.Body.Add(new XElement(Namespaces.Xhtml + "p", $"%{field.Name}%"));
            }

            return doc;
        }

        public static void SaveRenderingLayout(string formName, XhtmlDocument layout)
        {
            using (var writer = ResourceFacade.GetResourceWriter())
            {
                var key = GetKey(formName);

                writer.AddResource(key, layout.ToString());
            }
        }

        private static string GetKey(string formName)
        {
            return Localization.GenerateKey(formName, "RenderingLayout");
        }
    }
}
