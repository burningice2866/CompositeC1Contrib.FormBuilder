using System;
using System.Collections.Generic;
using System.Linq;

namespace CompositeC1Contrib.FormBuilder
{
    public class POCOFormModelsProvider : IFormModelsProvider
    {
        private static readonly IDictionary<string, Type> Types = new Dictionary<string, Type>();
        private static readonly IDictionary<IPOCOForm, FormModel> Models = new Dictionary<IPOCOForm, FormModel>();

        static POCOFormModelsProvider()
        {
            var formTypes = GetFormTypes();

            foreach (var type in formTypes)
            {
                var instance = (IPOCOForm)Activator.CreateInstance(type);
                var model = POCOFormsFacade.FromInstance(instance);

                Types.Add(model.Name, type);
                Models.Add(instance, model);
            }
        }

        public static IDictionary<IPOCOForm, FormModel> GetFormsAndModels()
        {
            return Models;
        }

        private static IEnumerable<Type> GetFormTypes()
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

        public IEnumerable<FormModel> GetModels()
        {
            return Models.Select(e => e.Value);
        }
    }
}
