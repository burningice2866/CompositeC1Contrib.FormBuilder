namespace CompositeC1Contrib.FormBuilder
{
    public class FormWizardStep
    {
        public string Name { get; set; }
        public string FormName { get; set; }
        public int LocalOrdering { get; set; }
        public string NextButtonLabel { get; set; }
        public string PreviousButtonLabel { get; set; }

        private IFormModel _formModel;
        public IFormModel FormModel
        {
            get { return _formModel ?? (_formModel = FormModelsFacade.GetModel(FormName)); }
        }
    }
}
