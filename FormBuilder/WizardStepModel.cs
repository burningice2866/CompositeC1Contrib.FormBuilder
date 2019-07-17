namespace CompositeC1Contrib.FormBuilder
{
    public class WizardStepModel
    {
        public string Name { get; set; }
        public string FormName { get; set; }
        public string Label { get; set; }
        public int LocalOrdering { get; set; }
        public string NextButtonLabel { get; set; }
        public string PreviousButtonLabel { get; set; }

        private IModel _formModel;
        public IModel FormModel => _formModel ?? (_formModel = ModelsFacade.GetModel(FormName));
    }
}
