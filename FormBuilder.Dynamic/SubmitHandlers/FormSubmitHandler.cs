using System;

namespace CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers
{
    [Serializable]
    public abstract class FormSubmitHandler
    {
        public string Name { get; set; }
        public abstract void Submit(FormModel model);
    }
}
