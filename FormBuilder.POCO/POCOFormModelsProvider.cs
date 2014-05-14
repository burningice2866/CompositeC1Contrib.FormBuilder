using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Web;

namespace CompositeC1Contrib.FormBuilder
{
    [Export(typeof(IFormModelsProvider))]
    public class POCOFormModelsProvider : IFormModelsProvider
    {
        private static readonly IDictionary<string, Type> Types = new Dictionary<string, Type>();
        private static readonly IDictionary<IPOCOForm, FormModel> Models = new Dictionary<IPOCOForm, FormModel>();

        static POCOFormModelsProvider()
        {
            var forms = GetForms();

            foreach (var instance in forms)
            {
                var model = POCOFormsFacade.FromInstance(instance);

                Types.Add(model.Name, instance.GetType());
                Models.Add(instance, model);
            }
        }

        public static IDictionary<IPOCOForm, FormModel> GetFormsAndModels()
        {
            return Models;
        }

        public IEnumerable<IFormModel> GetModels()
        {
            return Models.Select(e => e.Value);
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
