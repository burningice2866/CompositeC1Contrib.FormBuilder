using System;
using System.ComponentModel.Composition;

namespace CompositeC1Contrib.FormBuilder.Data
{
    [Export(typeof(IValueMapper))]
    public class GuidValueMapper : IValueMapper
    {
        public Type ValueMapperFor => typeof(Guid);

        public void MapValue(FormField field, string value)
        {
            Guid.TryParse(value, out var val);

            field.Value = val;
        }
    }

    [Export(typeof(IValueMapper))]
    public class NullableGuidValueMapper : IValueMapper
    {
        public Type ValueMapperFor => typeof(Guid?);

        public void MapValue(FormField field, string value)
        {
            if (Guid.TryParse(value, out var val))
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