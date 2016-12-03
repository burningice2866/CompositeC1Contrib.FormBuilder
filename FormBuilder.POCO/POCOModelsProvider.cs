using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.FormBuilder.FunctionProviders;
using CompositeC1Contrib.FormBuilder.Web;

namespace CompositeC1Contrib.FormBuilder
{
    [Export(typeof(IModelsProvider))]
    public class POCOModelsProvider : IModelsProvider
    {
        private readonly IDictionary<string, Tuple<IModel, Type>> _models = new Dictionary<string, Tuple<IModel, Type>>();

        public POCOModelsProvider()
        {
            var formTypes = GetFormTypes();

            foreach (var type in formTypes)
            {
                var model = POCOModelsFacade.FromType(type);

                if (_models.ContainsKey(model.Name))
                {
                    throw new InvalidOperationException(String.Format("Form '{0}' has already been added", model.Name));
                }

                _models.Add(model.Name, Tuple.Create(model, type));
            }
        }

        public Type GetTypeForModel(IModel model)
        {
            return _models[model.Name].Item2;
        }

        public IEnumerable<ProviderModelContainer> GetModels()
        {
            return _models.Select(m => new ProviderModelContainer
            {
                Source = typeof(POCOModelsProvider),
                Type = "StaticForm",
                Model = m.Value.Item1,
                Function = new StandardFormFunction<POCOFormBuilderRequestContext>(m.Value.Item1)
            });
        }

        private static IEnumerable<Type> GetFormTypes()
        {
            return CompositionContainerFacade.GetExportedTypes<IPOCOForm>(b =>
            {
                var partBuilder = b.ForTypesMatching(t => t.IsClass && !t.IsAbstract && typeof(IPOCOForm).IsAssignableFrom(t));

                partBuilder.Export<IPOCOForm>();

            }).Select(t => t.UnderlyingSystemType);
        }
    }
}
