using System.Xml.Linq;

namespace CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers
{
    public abstract class FormSubmitHandler
    {
        public string Name { get; set; }
        public abstract void Submit(IFormModel model);

        public virtual void Load(IDynamicFormDefinition model, XElement handler)
        {
            Name = handler.Attribute("Name").Value;
        }

        public virtual void Save(IDynamicFormDefinition definition, XElement handler) { }
        public virtual void Delete(IDynamicFormDefinition definition) { }
    }
}
