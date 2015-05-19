using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Web;

namespace CompositeC1Contrib.FormBuilder.POCO
{
    [Export(typeof(IFormModelsProvider))]
    public class POCOFormModelsProvider : IFormModelsProvider
    {
        private readonly IDictionary<string, Type> _types = new Dictionary<string, Type>();
        private readonly IDictionary<IPOCOForm, FormModel> _models = new Dictionary<IPOCOForm, FormModel>();

        public POCOFormModelsProvider()
        {
            var forms = GetForms();

            foreach (var instance in forms)
            {
                var model = POCOFormsFacade.FromInstance(instance);

                if (_types.ContainsKey(model.Name))
                {
                    throw new InvalidOperationException(String.Format("Form '{0}' has already been added", model.Name));
                }

                _types.Add(model.Name, instance.GetType());
                _models.Add(instance, model);
            }
        }

        public IDictionary<IPOCOForm, FormModel> GetFormsAndModels()
        {
            return _models;
        }

        public IEnumerable<ProviderModelContainer> GetModels()
        {
            return _models.Select(e => new ProviderModelContainer
            {
                Source = typeof(POCOFormModelsProvider),
                Type = "StaticForm",
                Model = e.Value
            });
        }

        private static IEnumerable<IPOCOForm> GetForms()
        {
            var registrationBuilder = new RegistrationBuilder();
            registrationBuilder.ForTypesMatching(t => t.IsClass && !t.IsAbstract && typeof(IPOCOForm).IsAssignableFrom(t)).Export<IPOCOForm>();

            var batch = new CompositionBatch();
            var catalog = new SafeDirectoryCatalog(HttpRuntime.BinDirectory, registrationBuilder);
            var container = new CompositionContainer(catalog);

            container.Compose(batch);

            return container.GetExportedValues<IPOCOForm>();
        }
    }
}
