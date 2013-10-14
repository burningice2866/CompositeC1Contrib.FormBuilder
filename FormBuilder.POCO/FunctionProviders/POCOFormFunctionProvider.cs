using System;
using System.Collections.Generic;
using System.Linq;

using Composite.Functions;
using Composite.Functions.Plugins.FunctionProvider;

using CompositeC1Contrib.FormBuilder.Attributes;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class POCOFormFunctionProvider : IFunctionProvider
    {
        public FunctionNotifier FunctionNotifier { private get; set; }

        public IEnumerable<IFunction> Functions
        {
            get
            {
                var formTypes = GetFormTypes();
                foreach (var type in formTypes)
                {
                    string functionName = "Forms." + type.Name;

                    var formName = type.GetCustomAttributes(typeof(FormNameAttribute), false).FirstOrDefault() as FormNameAttribute;
                    if (formName != null)
                    {
                        functionName = formName.FullName;
                    }                    

                    IFunction function = null;
                    if (!FunctionFacade.TryGetFunction(out function, functionName))
                    {
                        yield return new POCOFormFunction(type);
                    }
                }
            }
        }

        public static IEnumerable<Type> GetFormTypes()
        {
            var returnList = new List<Type>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in assemblies)
            {
                try
                {
                    var types = asm.GetTypes().Where(t => t.GetCustomAttributes(true).OfType<FormNameAttribute>().Any());

                    returnList.AddRange(types);
                }
                catch { }
            }

            return returnList;
        }
    }
}
