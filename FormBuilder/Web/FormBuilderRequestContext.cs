namespace CompositeC1Contrib.FormBuilder.Web
{
    public abstract class FormBuilderRequestContext : BaseFormBuilderRequestContext<FormModel>
    {
        protected FormBuilderRequestContext(string name) : base(name) { }

        public override void Submit()
        {
            if (HttpContext.IsDebuggingEnabled)
            {
                SaveFormSubmitFacade.SaveSubmitDebug(RenderingModel);
            }

            base.Submit();
        }
    }
}
