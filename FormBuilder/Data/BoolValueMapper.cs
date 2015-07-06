using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

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

    [Export(typeof(IValueMapper))]
    public class EnumerableBoolValueMapper : IValueMapper
    {
        public Type ValueMapperFor
        {
            get { return typeof(IEnumerable<bool>); }
        }

        public void MapValue(FormField field, string value)
        {
            field.Value = value.Split(',').Select(s => s == "on");
        }
    }
}