using Composite.Core.Xml;

using CompositeC1Contrib.FormBuilder.Web;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class FormWizardFunction<T> : BaseFormFunction<T, Wizard> where T : WizardRequestContext
    {
        protected override string StandardFormExecutor
        {
            get { return "FormBuilder.StandardFormWizardExecutor"; }
        }

        public FormWizardFunction(string name) : this(name, null, null) { }
        public FormWizardFunction(string name, XhtmlDocument introText, XhtmlDocument successResponse) : base(name, introText, successResponse) { }
    }
}
