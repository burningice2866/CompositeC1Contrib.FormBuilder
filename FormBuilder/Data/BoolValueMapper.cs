using System;
using System.ComponentModel.Composition;

namespace CompositeC1Contrib.FormBuilder.Data
{
    [Export(typeof(IValueMapper))]
    public class BoolValueMapper : IValueMapper
    {
        public Type ValueMapperFor
        {
            get { return typeof(bool); }
        }

        public void MapValue(FormField field, string value)
        {
            bool b;

            if (value == "on")
            {
                b = true;
            }
            else
            {
                bool.TryParse(value, out b);
            }

            field.Value = b;
        }
    }
}