using CompositeC1Contrib.FormBuilder.Web;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class FormWizardFunction<T> : BaseFormFunction<T, Wizard> where T : WizardRequestContext
    {
        protected override string StandardFormExecutor => "FormBuilder.StandardFormWizardExecutor";

        public FormWizardFunction(IModel form) : base(form) { }
    }
}
