using Composite.Data;

namespace CompositeC1Contrib.FormBuilder.Data.Types
{
    public class ModelReference : IModelReference
    {
        public DataSourceId DataSourceId { get; private set; }
        public string Name { get; private set; }

        public ModelReference(IModel formModel, DataSourceId dataSourceId)
        {
            Name = formModel.Name;
            DataSourceId = dataSourceId;
        }
    }
}