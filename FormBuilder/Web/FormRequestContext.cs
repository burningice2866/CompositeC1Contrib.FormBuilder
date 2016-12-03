using System;

namespace CompositeC1Contrib.FormBuilder.Web
{
    public abstract class FormRequestContext : BaseFormBuilderRequestContext<Form>
    {
        protected FormRequestContext(IModel model)
            : base(model)
        {
            var formModel = model as FormModel;
            if (formModel == null)
            {
                throw new ArgumentException($"Supplied form was not of the correct type, expected '{typeof(FormModel).FullName}' but got '{model.GetType().FullName}");
            }

            ModelInstance = new Form(formModel);
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
