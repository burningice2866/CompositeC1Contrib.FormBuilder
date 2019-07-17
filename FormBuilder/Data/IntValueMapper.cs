using System;
using System.ComponentModel.Composition;

namespace CompositeC1Contrib.FormBuilder.Data
{
    [Export(typeof(IValueMapper))]
    public class IntValueMapper : IValueMapper
    {
        public Type ValueMapperFor => typeof(int);

        public void MapValue(FormField field, string value)
        {
            int.TryParse(value, out var val);

            field.Value = val;
        }
    }

    [Export(typeof(IValueMapper))]
    public class NullableIntValueMapper : IValueMapper
    {
        public Type ValueMapperFor => typeof(int?);

        public void MapValue(FormField field, string value)
        {
            if (int.TryParse(value, out var val))
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