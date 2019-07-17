using System;
using System.ComponentModel.Composition;

namespace CompositeC1Contrib.FormBuilder.Data
{
    [Export(typeof(IValueMapper))]
    public class DateTimeValueMapper : IValueMapper
    {
        public Type ValueMapperFor => typeof(DateTime);

        public void MapValue(FormField field, string value)
        {
            DateTime val;
            DateTime.TryParse(value, out val);

            field.Value = val;
        }
    }

    [Export(typeof(IValueMapper))]
    public class NullableDateTimeValueMapper : IValueMapper
    {
        public Type ValueMapperFor => typeof(DateTime?);

        public void MapValue(FormField field, string value)
        {
            if (DateTime.TryParse(value, out var val))
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