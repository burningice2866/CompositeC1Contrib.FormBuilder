using System.Linq;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    public class StringBasedDataSourceAttribute : DataSourceAttribute
    {
        private string[] _values;

        public StringBasedDataSourceAttribute(string[] values)
        {
            _values = values;
        }

        public override object GetData()
        {
            return _values.ToDictionary(s => s);
        }
    }
}
