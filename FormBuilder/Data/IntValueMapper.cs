using System;
using System.ComponentModel.Composition;

namespace CompositeC1Contrib.FormBuilder.Data
{
    [Export(typeof(IValueMapper))]
    public class IntValueMapper : IValueMapper
    {
        public Type ValueMapperFor
        {
            get { return typeof(int); }
        }

        public void MapValue(FormField field, string value)
        {
            int val;
            int.TryParse(value, out val);

            field.Value = val;
        }
    }
}