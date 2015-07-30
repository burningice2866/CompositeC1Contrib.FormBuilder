using CompositeC1Contrib.FormBuilder.Dynamic;

namespace CompositeC1Contrib.FormBuilder.Web
{
    public class DynamicFormBuilderRequestContext : FormRequestContext
    {
        public DynamicFormBuilderRequestContext(string name) : base(name) { }

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
