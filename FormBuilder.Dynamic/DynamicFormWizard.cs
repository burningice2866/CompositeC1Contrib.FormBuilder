using System.Collections.Generic;

using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public class DynamicFormWizard : FormWizard, IDynamicFormDefinition
    {
        public IList<FormSubmitHandler> SubmitHandlers { get; private set; }
        public IFormSettings Settings { get; set; }

        public DynamicFormWizard()
        {
            SubmitHandlers = new List<FormSubmitHandler>();
        }

        public override void Submit()
        {
            foreach (var handler in SubmitHandlers)
            {
                handler.Submit(this);
            }

            base.Submit();
        }
    }
}
