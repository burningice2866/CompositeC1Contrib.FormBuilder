namespace CompositeC1Contrib.FormBuilder.Web
{
    public abstract class WizardRequestContext : BaseFormBuilderRequestContext<Wizard>
    {
        public Wizard Wizard
        {
            get { return ModelInstance; }
        }

        protected WizardRequestContext(string name)
            : base(name)
        {
            var model = ModelsFacade.GetModel<WizardModel>(name);

            ModelInstance = new Wizard(model);
        }
    }
}
