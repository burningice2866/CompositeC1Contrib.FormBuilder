using System.Linq;

namespace CompositeC1Contrib.FormBuilder
{
    public class POCOFormBuilderRequestContext : FormBuilderRequestContext
    {
        private FormModel _model;
        private IPOCOForm _instance;

        public override FormModel RenderingModel
        {
            get { return _model; }
        }

        public POCOFormBuilderRequestContext(string formName)
            : base(formName)
        {
            _instance = POCOFormModelsProvider.GetInstanceByName(formName);
            _model = POCOFormsFacade.FromInstance(_instance, null);
        }

        public override void SetDefaultValues()
        {
            POCOFormsFacade.SetDefaultValues(_instance, _model);
        }

        public override void OnMappedValues()
        {
            POCOFormsFacade.SetDefaultValues(_instance, _model);
        }

        public override void OnSubmit()
        {
            _instance.Submit();
        }
    }
}
