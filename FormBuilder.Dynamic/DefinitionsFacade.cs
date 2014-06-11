using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public class DefinitionsFacade
    {
        private static readonly IList<Action> FormChangeNotifications = new List<Action>();

        public static void SubscribeToFormChanges(Action notify)
        {
            FormChangeNotifications.Add(notify);
        }

        public static IEnumerable<IDynamicFormDefinition> GetDefinitions()
        {
            var files = Directory.GetFiles(FormModelsFacade.RootPath, "DynamicDefinition.xml", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var folder = Path.GetDirectoryName(file);
                var name = new DirectoryInfo(folder).Name;
                var xml = XElement.Load(file);

                var serializer = XmlDefinitionSerializer.GetSerializer(xml);

                yield return serializer.Load(name, xml);
            }
        }

        public static IDynamicFormDefinition GetDefinition(string name)
        {
            var file = Path.Combine(FormModelsFacade.RootPath, name, "DynamicDefinition.xml");
            if (!File.Exists(file))
            {
                return null;
            }

            var xml = XElement.Load(file);
            var seralizer = XmlDefinitionSerializer.GetSerializer(xml);

            return seralizer.Load(name, xml);
        }

        public static void Save(IDynamicFormDefinition definition)
        {
            var serializer = XmlDefinitionSerializer.GetSerializer(definition.Name);

            serializer.Save(definition);
        }

        public static void Delete(IDynamicFormDefinition definition)
        {
            var dir = Path.Combine(FormModelsFacade.RootPath, definition.Name);

            Directory.Delete(dir, true);

            foreach (var submithandler in definition.SubmitHandlers)
            {
                submithandler.Delete(definition);
            }

            NotifyFormChanges();
        }

        public static void Copy(IDynamicFormDefinition definition, string newName)
        {
            var def = GetDefinition(definition.Name);

            def.Name = newName;

            var serializer = XmlDefinitionSerializer.GetSerializer(definition.Name);

            serializer.Save(def);
        }

        public static void NotifyFormChanges()
        {
            foreach (var action in FormChangeNotifications)
            {
                action();
            }
        }
    }
}
