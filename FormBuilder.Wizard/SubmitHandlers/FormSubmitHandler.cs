using System;

using CompositeC1Contrib.FormBuilder.Wizard.Web;

namespace CompositeC1Contrib.FormBuilder.Wizard.SubmitHandlers
{
    [Serializable]
    public abstract class FormSubmitHandler
    {
        public string Name { get; set; }
        public abstract void Submit(FormWizardRequestContext context);
    }
}
