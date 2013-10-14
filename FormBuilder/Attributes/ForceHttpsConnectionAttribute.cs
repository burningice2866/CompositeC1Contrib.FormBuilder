using System;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ForceHttpsConnectionAttribute : Attribute
    {
        public bool ForceHttps { get; private set; }

        public ForceHttpsConnectionAttribute(bool forceHttps)
        {
            ForceHttps = forceHttps;
        }
    }
}
