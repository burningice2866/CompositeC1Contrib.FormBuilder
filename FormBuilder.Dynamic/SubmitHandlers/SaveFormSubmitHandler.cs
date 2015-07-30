using System;
using System.Xml.Linq;

namespace CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers
{
    [Serializable]
    public class SaveFormSubmitHandler : FormSubmitHandler
    {
        public bool IncludeAttachments { get; set; }

        public override void Submit(IModelInstance instance)
        {
            SaveSubmitFacade.SaveSubmit(instance, IncludeAttachments);
        }

        public override void Save(IDynamicDefinition definition, XElement handler)
        {
            handler.Add(new XAttribute("IncludeAttachments", IncludeAttachments));
        }
    }
}