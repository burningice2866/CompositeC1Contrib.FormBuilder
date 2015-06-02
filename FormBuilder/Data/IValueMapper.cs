using System;

namespace CompositeC1Contrib.FormBuilder.Data
{
    public interface IValueMapper
    {
        Type ValueMapperFor { get; }

        void MapValue(FormField field, string value);
    }
}
