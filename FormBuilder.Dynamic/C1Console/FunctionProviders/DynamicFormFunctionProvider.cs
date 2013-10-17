using System;
using System.Collections.Generic;
using System.Linq;

using Composite.Functions;
using Composite.Functions.Plugins.FunctionProvider;

using CompositeC1Contrib.FormBuilder.Attributes;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class DynamicFormFunctionProvider : IFunctionProvider
    {
        public FunctionNotifier FunctionNotifier { private get; set; }

        public IEnumerable<IFunction> Functions
        {
            get
            {
                yield return new DynamicFormFunction("Forms.Dynamic.Test");
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
