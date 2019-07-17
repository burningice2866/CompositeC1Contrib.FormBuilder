using System.Collections.Generic;
using System.Linq;

using Composite.Functions;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public class DynamicFormsFacade
    {
        public static DynamicFormDefinition GetFormByName(string name)
        {
            return (DynamicFormDefinition)DefinitionsFacade.GetDefinition(name);
        }

        public static IEnumerable<DynamicFormDefinition> GetFormDefinitions()
        {
            return DefinitionsFacade.GetDefinitions().OfType<DynamicFormDefinition>();
        }

        public static void SetDefaultValues(IModelInstance instance)
        {
            var def = GetFormByName(instance.Name);

            foreach (var field in instance.Fields)
            {
                if (!def.DefaultValues.TryGetValue(field.Name, out var defaultValueSetter))
                {
                    continue;
                }

                var runtimeTree = FunctionFacade.BuildTree(defaultValueSetter);

                field.Value = runtimeTree.GetValue();
            }
        }

        public static void SaveForm(IDynamicDefinition definition)
        {
            var serializer = new FormXmlSerializer();

            serializer.Save(definition);
        }
    }
}
