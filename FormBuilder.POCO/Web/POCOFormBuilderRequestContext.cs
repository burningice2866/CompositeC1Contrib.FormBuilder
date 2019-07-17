using System.Linq;
using System.Reflection;

namespace CompositeC1Contrib.FormBuilder.Web
{
    public class POCOFormBuilderRequestContext : FormRequestContext
    {
        public IPOCOForm Instance { get; }

        public POCOFormBuilderRequestContext(IModel model) : base(model)
        {
            Instance = (IPOCOForm)ModelInstance.FormData["PocoInstance"];
        }

        protected override void OnMappedValues()
        {
            POCOModelsFacade.MapValues(Instance, ModelInstance);
        }

        public override void Submit()
        {
            var importSubmittedValuesProperties = Instance.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(p => p.CanWrite && p.GetCustomAttribute<ImportSubmittedValuesAttribute>() != null);

            foreach (var prop in importSubmittedValuesProperties)
            {
                prop.SetValue(Instance, ModelInstance.SubmittedValues);
            }

            var importSubmittedValuesFields = Instance.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.GetCustomAttribute<ImportSubmittedValuesAttribute>() != null);

            foreach (var field in importSubmittedValuesFields)
            {
                field.SetValue(Instance, ModelInstance.SubmittedValues);
            }

            Instance.Submit();

            base.Submit();
        }
    }
}
