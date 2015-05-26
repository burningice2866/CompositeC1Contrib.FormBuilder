namespace CompositeC1Contrib.FormBuilder.Web
{
    public abstract class FormWizardRequestContext : BaseFormBuilderRequestContext<FormWizard>
    {
        public FormWizard Wizard
        {
            get { return RenderingModel; }
        }

        protected FormWizardRequestContext(string name) : base(name) { }

        public override void Submit()
        {
            Wizard.Submit();

            base.Submit();
        }
    }
}
