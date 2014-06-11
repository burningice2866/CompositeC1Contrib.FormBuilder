using CompositeC1Contrib.FormBuilder.Dynamic;

namespace CompositeC1Contrib.FormBuilder.Web
{
    public class DynamicFormWizardRequestContext : FormWizardRequestContext
    {
        private readonly DynamicFormWizard _wizard;

        public override FormWizard RenderingModel
        {
            get { return _wizard; }
        }

        public DynamicFormWizardRequestContext(string name) : base(name)
        {
            _wizard = DynamicFormWizardsFacade.GetWizard(name);
        }
    }
}
