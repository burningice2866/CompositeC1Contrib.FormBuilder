using CompositeC1Contrib.FormBuilder.Dynamic;

namespace CompositeC1Contrib.FormBuilder.Web
{
    public class DynamicFormBuilderRequestContext : FormRequestContext
    {
        public DynamicFormBuilderRequestContext(IModel model) : base(model) { }

        public override void Submit()
        {
            var def = DynamicFormsFacade.GetFormByName(ModelInstance.Name);

            foreach (var handler in def.SubmitHandlers)
            {
                handler.Submit(ModelInstance);
            }

            base.Submit();
        }
    }
}
