using System;
using System.Xml.Linq;

namespace CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers
{
    [Serializable]
    public class SaveFormSubmitHandler : FormSubmitHandler
    {
        public bool IncludeAttachments { get; set; }

        public override void Submit(IFormModel model)
        {
            SaveFormSubmitFacade.SaveSubmit(model, IncludeAttachments);
        }

        public override void Save(IDynamicFormDefinition definition, XElement handler)
        {
            handler.Add(new XAttribute("IncludeAttachments", IncludeAttachments));
        }
    }
}