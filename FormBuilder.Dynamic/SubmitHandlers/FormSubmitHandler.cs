using System.Xml.Linq;

namespace CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers
{
    public abstract class FormSubmitHandler
    {
        public string Name { get; set; }
        public abstract void Submit(IModelInstance instance);

        public virtual void Load(IDynamicDefinition model, XElement handler)
        {
            Name = handler.Attribute("Name").Value;
        }

        public virtual void Save(IDynamicDefinition definition, XElement handler) { }
        public virtual void Delete(IDynamicDefinition definition) { }
    }
}
