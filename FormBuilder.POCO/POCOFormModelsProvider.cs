using System;
using System.Collections.Generic;
using System.Linq;

namespace CompositeC1Contrib.FormBuilder
{
    public class POCOFormModelsProvider : IFormModelsProvider
    {
        public static IDictionary<IPOCOForm, FormModel> GetModels()
        {
            var dict = new Dictionary<IPOCOForm, FormModel>();

            var formTypes = GetFormTypes();
            foreach (var type in formTypes)
            {
                var instance = (IPOCOForm)Activator.CreateInstance(type);
                var model = POCOFormsFacade.FromInstance(instance, null);

                dict.Add(instance, model);
            }

            return dict;
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
