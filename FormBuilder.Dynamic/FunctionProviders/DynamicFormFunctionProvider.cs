using System.Collections.Generic;
using System.Xml.Linq;

using Composite.Functions;
using Composite.Functions.Plugins.FunctionProvider;

using CompositeC1Contrib.FormBuilder.Dynamic;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class DynamicFormFunctionProvider : IFunctionProvider
    {
        public FunctionNotifier FunctionNotifier { private get; set; }

        public IEnumerable<IFunction> Functions
        {
            get
            {
                var definitions = DynamicFormsFacade.GetFormDefinitions();
                foreach (var def in definitions)
                {
                    IFunction function = null;
                    if (!FunctionFacade.TryGetFunction(out function, def.Name))
                    {
                        yield return new StandardFormFunction(def.Model)
                        {
                            OnSubmit = () =>
                            {
                                foreach (var handler in def.SubmitHandlers)
                                {
                                    handler.Submit(def.Model);
                                }
                            },

                            SetDefaultValues = (m) =>
                            {
                                foreach (var field in m.Fields)
                                {
                                    XElement defaultValueSetter;
                                    if (def.DefaultValues.TryGetValue(field.Name, out defaultValueSetter))
                                    {
                                        var runtimeTree = FunctionFacade.BuildTree(defaultValueSetter);

                                        field.Value = runtimeTree.GetValue();
                                    }
                                }
                            },

                            OverrideFormExecutor = def.FormExecutor
                        };
                    }
                }
            }
        }

        public DynamicFormFunctionProvider()
        {
            DynamicFormsFacade.SubscribeToFormChanges(() =>
            {
                if (FunctionNotifier != null)
                {
                    FunctionNotifier.FunctionsUpdated();
                }
            });
        }
    }
}
