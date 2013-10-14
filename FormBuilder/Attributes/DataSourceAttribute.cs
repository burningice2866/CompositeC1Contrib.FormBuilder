using System;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public abstract class DataSourceAttribute : Attribute
    {
        public abstract object GetData();

    }
}
