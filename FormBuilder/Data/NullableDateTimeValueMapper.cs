using System;
using System.ComponentModel.Composition;

namespace CompositeC1Contrib.FormBuilder.Data
{
    [Export(typeof(IValueMapper))]
    public class NullableDateTimeValueMapper : IValueMapper
    {
        public Type ValueMapperFor
        {
            get { return typeof(DateTime?); }
        }

        public void MapValue(FormField field, string value)
        {
            DateTime val;
            if (DateTime.TryParse(value, out val))
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