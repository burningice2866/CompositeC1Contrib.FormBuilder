using Composite.Data;

namespace CompositeC1Contrib.FormBuilder.Data.Types
{
    public class FormData : IForm
    {
        public DataSourceId DataSourceId { get; private set; }
        public string Name { get; private set; }

        public FormData(IFormModel formModel, DataSourceId dataSourceId)
        {
            Name = formModel.Name;
            DataSourceId = dataSourceId;
        }
    }
}