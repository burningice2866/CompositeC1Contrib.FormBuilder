using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace CompositeC1Contrib.FormBuilder.Data
{
    [Export(typeof(IValueMapper))]
    public class EnumerableStringValueMapper : IValueMapper
    {
        public Type ValueMapperFor
        {
            get { return typeof(IEnumerable<string>); }
        }

        public void MapValue(FormField field, string value)
        {
            field.Value = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}