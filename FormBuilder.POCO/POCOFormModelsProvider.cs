using System;
using System.Collections.Generic;
using System.Linq;

namespace CompositeC1Contrib.FormBuilder
{
    public class POCOFormModelsProvider : IFormModelsProvider
    {
        private static IDictionary<string, Type> _modelTypes = new Dictionary<string, Type>();

        static POCOFormModelsProvider()
        {
            var formTypes = GetFormTypes();
            foreach (var type in formTypes)
            {
                var instance = (IPOCOForm)Activator.CreateInstance(type);
                var model = POCOFormsFacade.FromInstance(instance, null);

                _modelTypes.Add(model.Name, type);
            }
        }

        public static IDictionary<IPOCOForm, FormModel> GetModels()
        {
            var dict = new Dictionary<IPOCOForm, FormModel>();

            foreach (var type in _modelTypes.Values)
            {
                var instance = (IPOCOForm)Activator.CreateInstance(type);
                var model = POCOFormsFacade.FromInstance(instance, null);

                dict.Add(instance, model);
            }

            return dict;
        }

        public static IPOCOForm GetInstanceByName(string name)
        {
            var type = _modelTypes[name];
            var instance = (IPOCOForm)Activator.CreateInstance(type);

            return instance;
        }

        public static IEnumerable<Type> GetFormTypes()
        {
            var returnList = new List<Type>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in assemblies)
            {
                try
                {
                    var types = asm.GetTypes()
                        .Where(t => t.IsClass && !t.IsAbstract && typeof(IPOCOForm).IsAssignableFrom(t));

                    returnList.AddRange(types);
                }
                catch { }
            }

            return returnList;
        }

        IEnumerable<FormModel> IFormModelsProvider.GetModels()
        {
            return GetModels().Select(e => e.Value);
        }
    }
}
