using System;

namespace CompositeC1Contrib.FormBuilder.Web
{
    public abstract class WizardRequestContext : BaseFormBuilderRequestContext<Wizard>
    {
        public Wizard Wizard => ModelInstance;

        protected WizardRequestContext(IModel model)
            : base(model)
        {
            var wizardModel = model as WizardModel;
            if (wizardModel == null)
            {
                throw new ArgumentException($"Supplied form was not of the correct type, expected '{typeof(WizardModel).FullName}' but got '{model.GetType().FullName}");
            }

            ModelInstance = new Wizard(wizardModel);
        }
    }
}
