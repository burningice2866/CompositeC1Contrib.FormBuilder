using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

using Composite.Core.IO;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public static class DefinitionsFacade
    {
        public static IEnumerable<IDynamicDefinition> GetDefinitions()
        {
            var files = C1Directory.GetFiles(ModelsFacade.RootPath, "DynamicDefinition.xml", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var folder = Path.GetDirectoryName(file);
                var name = new C1DirectoryInfo(folder).Name;
                var xml = XElement.Load(file);

                var serializer = XmlDefinitionSerializer.GetSerializer(xml);

                yield return serializer.Load(name, xml);
            }
        }

        public static IDynamicDefinition GetDefinition(string name)
        {
            var file = Path.Combine(ModelsFacade.RootPath, name, "DynamicDefinition.xml");
            if (!C1File.Exists(file))
            {
                return null;
            }

            var xml = XElement.Load(file);
            var serializer = XmlDefinitionSerializer.GetSerializer(xml);

            return serializer.Load(name, xml);
        }

        public static void Save(IDynamicDefinition definition)
        {
            var serializer = XmlDefinitionSerializer.GetSerializer(definition.Name);

            serializer.Save(definition);
        }

        public static void Delete(IDynamicDefinition definition)
        {
            var dir = Path.Combine(ModelsFacade.RootPath, definition.Name);

            C1Directory.Delete(dir, true);

            foreach (var submitHandler in definition.SubmitHandlers)
            {
                submitHandler.Delete(definition);
            }

            ModelsFacade.NotifyFormChanges();
        }

        public static void Copy(IDynamicDefinition definition, string newName)
        {
            var def = GetDefinition(definition.Name);

            def.Name = newName;

            var serializer = XmlDefinitionSerializer.GetSerializer(definition.Name);

            serializer.Save(def);
        }
    }
}
