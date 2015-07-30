using CompositeC1Contrib.FormBuilder.Dynamic;

namespace CompositeC1Contrib.FormBuilder.Web
{
    public class DynamicFormWizardRequestContext : WizardRequestContext
    {
        public DynamicFormWizardRequestContext(string name) : base(name) { }

        public override void Submit()
        {
            var def = DynamicWizardsFacade.GetWizard(ModelInstance.Name);

            foreach (var handler in def.SubmitHandlers)
            {
                handler.Submit(ModelInstance);
            }

            base.Submit();
        }
    }
}
