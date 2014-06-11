using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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

        public static void SetDefaultValues(IFormModel model)
        {
            var def = GetFormByName(model.Name);

            foreach (var field in model.Fields)
            {
                XElement defaultValueSetter;
                if (!def.DefaultValues.TryGetValue(field.Name, out defaultValueSetter))
                {
                    continue;
                }

                var runtimeTree = FunctionFacade.BuildTree(defaultValueSetter);

                field.Value = runtimeTree.GetValue();
            }
        }

        public static void SaveForm(DynamicFormDefinition definition)
        {
            var serializer = new FormXmlSerializer();

            serializer.Save(definition);
        }
    }
}
