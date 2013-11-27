using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml.Linq;

using Composite.Core.Xml;

namespace CompositeC1Contrib.FormBuilder
{
    public static class FormModelsFacade
    {
        private static IList<IFormModelsProvider> _modelProviders = new List<IFormModelsProvider>();
        private static string _basePath = HostingEnvironment.MapPath("~/App_Data/FormBuilder/FormRenderingLayouts");

        static FormModelsFacade()
        {            
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }

            var asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in asms)
            {
                try
                {
                    var types = asm.GetTypes()
                        .Where(t => typeof(IFormModelsProvider).IsAssignableFrom(t) && !t.IsInterface);

                    foreach (var t in types)
                    {
                        var instance = (IFormModelsProvider)Activator.CreateInstance(t);

                        _modelProviders.Add(instance);
                    }
                }
                catch { }
            }
        }

        public static XhtmlDocument GetRenderingLayout(string formName)
        {
            var file = Path.Combine(_basePath, formName + ".xml");
            if (File.Exists(file))
            {
                var fileContent = File.ReadAllText(file);

                return XhtmlDocument.Parse(fileContent);
            }
            else
            {
                var doc = new XhtmlDocument();
                var model = FormModelsFacade.GetModels().Single(m => m.Name == formName);

                foreach (var field in model.Fields.Where(f => f.Label != null))
                {
                    doc.Body.Add(new XElement(Namespaces.Xhtml + "p", String.Format("%{0}%", field.Name)));
                }

                return doc;
            }
        }

        public static void SaveRenderingLayout(string formName, XhtmlDocument markup)
        {
            var file = Path.Combine(_basePath, formName + ".xml");

            if (markup.IsEmpty)
            {
                File.Delete(file);
            }
            else
            {
                File.WriteAllText(file, markup.ToString());
            }
        }

        public static IEnumerable<FormModel> GetModels() 
        {
            foreach (var provider in _modelProviders)
            {
                foreach (var model in provider.GetModels())
                {
                    yield return model;
                }
            }
        }
    }
}
