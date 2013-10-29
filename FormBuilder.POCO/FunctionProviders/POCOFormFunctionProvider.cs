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
                        var instance = (IPOCOForm)Activator.CreateInstance(type);
                        var model = POCOFormsFacade.FromInstance(instance, null);

                        yield return new StandardFormFunction(model)
                        {
                            OnSubmit = instance.Submit,
                            OnMappedValues = (m) => POCOFormsFacade.SetDefaultValues(instance, m),
                            SetDefaultValues = (m) => POCOFormsFacade.SetDefaultValues(instance, m)
                        };
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
