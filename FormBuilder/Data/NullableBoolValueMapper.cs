using System;
using System.ComponentModel.Composition;

namespace CompositeC1Contrib.FormBuilder.Data
{
    [Export(typeof(IValueMapper))]
    public class NullableBoolValueMapper : IValueMapper
    {
        public Type ValueMapperFor
        {
            get { return typeof(bool?); }
        }

        public void MapValue(FormField field, string value)
        {
            bool b;
            if (bool.TryParse(value, out b))
            {
                field.Value = b;
            }
            else
            {
                field.Value = null;
            }
        }
    }
}