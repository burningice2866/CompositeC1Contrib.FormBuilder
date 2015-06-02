using System;
using System.ComponentModel.Composition;

namespace CompositeC1Contrib.FormBuilder.Data
{
    [Export(typeof(IValueMapper))]
    public class NullableDecimalValueMapper : IValueMapper
    {
        public Type ValueMapperFor
        {
            get { return typeof(decimal?); }
        }

        public void MapValue(FormField field, string value)
        {
            decimal val;
            if (decimal.TryParse(value, out val))
            {
                field.Value = val;
            }
            else
            {
                field.Value = null;
            }
        }
    }
}