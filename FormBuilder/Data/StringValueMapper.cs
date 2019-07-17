using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace CompositeC1Contrib.FormBuilder.Data
{
    [Export(typeof(IValueMapper))]
    public class StringValueMapper : IValueMapper
    {
        public Type ValueMapperFor => typeof(string);

        public void MapValue(FormField field, string value)
        {
            field.Value = value;
        }
    }

    [Export(typeof(IValueMapper))]
    public class EnumerableStringValueMapper : IValueMapper
    {
        public Type ValueMapperFor => typeof(IEnumerable<string>);

        public void MapValue(FormField field, string value)
        {
            field.Value = value.Split(',');
        }
    }
}