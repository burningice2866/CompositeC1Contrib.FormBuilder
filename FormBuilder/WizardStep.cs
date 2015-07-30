namespace CompositeC1Contrib.FormBuilder
{
    public class WizardStep
    {
        public WizardStepModel Model { get; private set; }
        public Form Form { get; private set; }

        public string Name
        {
            get { return Model.Name; }
        }

        public string FormName
        {
            get { return Model.FormName; }
        }

        public string Label
        {
            get { return Model.Label; }
        }

        public int LocalOrdering
        {
            get { return Model.LocalOrdering; }
        }

        public string NextButtonLabel
        {
            get { return Model.NextButtonLabel; }
        }

        public string PreviousButtonLabel
        {
            get { return Model.PreviousButtonLabel; }
        }

        public WizardStep(WizardStepModel model)
        {
            var formModel = ModelsFacade.GetModel<FormModel>(model.FormName);

            Model = model;
            Form = new Form(formModel);
        }
    }
}
