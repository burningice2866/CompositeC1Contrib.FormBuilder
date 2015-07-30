namespace CompositeC1Contrib.FormBuilder.Web
{
    public abstract class FormRequestContext : BaseFormBuilderRequestContext<Form>
    {
        protected FormRequestContext(string name)
            : base(name)
        {
            var model = ModelsFacade.GetModel<FormModel>(name);

            ModelInstance = new Form(model);
        }

        public override void Submit()
        {
            if (HttpContext.IsDebuggingEnabled)
            {
                SaveSubmitFacade.SaveSubmitDebug(ModelInstance);
            }

            base.Submit();
        }
    }
}
