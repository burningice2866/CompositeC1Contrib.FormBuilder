namespace CompositeC1Contrib.FormBuilder
{
    public class WizardStep
    {
        public WizardStepModel Model { get; }
        public Form Form { get; }

        public string Name => Model.Name;

        public string FormName => Model.FormName;

        public string Label => Model.Label;

        public int LocalOrdering => Model.LocalOrdering;

        public string NextButtonLabel => Model.NextButtonLabel;

        public string PreviousButtonLabel => Model.PreviousButtonLabel;

        public WizardStep(WizardStepModel model)
        {
            var formModel = ModelsFacade.GetModel<FormModel>(model.FormName);

            Model = model;
            Form = new Form(formModel);
        }
    }
}
