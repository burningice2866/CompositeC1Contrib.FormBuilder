using System.Linq;
using System.Reflection;

using CompositeC1Contrib.FormBuilder.POCO;

namespace CompositeC1Contrib.FormBuilder.Web
{
    public class POCOFormBuilderRequestContext : FormBuilderRequestContext
    {
        private readonly FormModel _model;
        private readonly IPOCOForm _instance;

        public override FormModel RenderingModel
        {
            get { return _model; }
        }

        public POCOFormBuilderRequestContext(string formName, IPOCOForm instance)
            : base(formName)
        {
            _instance = instance;
            _model = POCOFormsFacade.FromInstance(_instance);
        }

        public override void OnMappedValues()
        {
            POCOFormsFacade.MapValues(_instance, _model);
        }

        public override void Submit()
        {
            var importSubmittedValuesProperties = _instance.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(p => p.CanWrite && p.GetCustomAttribute<ImportSubmittedValuesAttribute>() != null);

            foreach (var prop in importSubmittedValuesProperties)
            {
                prop.SetValue(_instance, _model.SubmittedValues);
            }

            var importSubmittedValuesFields = _instance.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.GetCustomAttribute<ImportSubmittedValuesAttribute>() != null);

            foreach (var field in importSubmittedValuesFields)
            {
                field.SetValue(_instance, _model.SubmittedValues);
            }

            _instance.Submit();
            
            base.Submit();
        }
    }
}
