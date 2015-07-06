using System;
using System.ComponentModel.Composition;

namespace CompositeC1Contrib.FormBuilder.Data
{
    [Export(typeof(IValueMapper))]
    public class GuidValueMapper : IValueMapper
    {
        public Type ValueMapperFor
        {
            get { return typeof(Guid); }
        }

        public void MapValue(FormField field, string value)
        {
            Guid val;
            Guid.TryParse(value, out val);

            field.Value = val;
        }
    }

    [Export(typeof(IValueMapper))]
    public class NullableGuidValueMapper : IValueMapper
    {
        public Type ValueMapperFor
        {
            get { return typeof(Guid?); }
        }

        public void MapValue(FormField field, string value)
        {
            Guid val;
            if (Guid.TryParse(value, out val))
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