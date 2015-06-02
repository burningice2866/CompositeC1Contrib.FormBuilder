using System;
using System.ComponentModel.Composition;

namespace CompositeC1Contrib.FormBuilder.Data
{
    [Export(typeof(IValueMapper))]
    public class StringValueMapper : IValueMapper
    {
        public Type ValueMapperFor
        {
            get { return typeof(string); }
        }

        public void MapValue(FormField field, string value)
        {
            field.Value = value;
        }
    }
}