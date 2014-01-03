namespace CompositeC1Contrib.FormBuilder
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
            _model = POCOFormsFacade.FromInstance(_instance, null);
        }

        public override void SetDefaultValues()
        {
            POCOFormsFacade.SetDefaultValues(_instance, _model);
        }

        public override void OnMappedValues()
        {
            POCOFormsFacade.MapValues(_instance, _model);
        }

        public override void OnSubmit()
        {
            _instance.Submit();
        }
    }
}
