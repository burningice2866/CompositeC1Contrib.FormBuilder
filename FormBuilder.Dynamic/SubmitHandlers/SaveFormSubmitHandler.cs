using System;

namespace CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers
{
    [Serializable]
    public class SaveFormSubmitHandler : FormSubmitHandler
    {
        public bool IncludeAttachments { get; set; }

        public override void Submit(FormModel model)
        {
            SaveFormSubmitFacade.SaveSubmit(model, IncludeAttachments);
        }
    }
}