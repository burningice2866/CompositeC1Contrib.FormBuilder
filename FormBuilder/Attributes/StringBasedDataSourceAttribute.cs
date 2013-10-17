using System.Linq;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    public class StringBasedDataSourceAttribute : DataSourceAttribute
    {
        public string[] Values { get; private set; }

        public StringBasedDataSourceAttribute(params string[] values)
        {
            Values = values;
        }

        public override object GetData()
        {
            return Values.ToDictionary(s => s);
        }
    }
}
