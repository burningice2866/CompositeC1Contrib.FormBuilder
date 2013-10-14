using System;
using System.Reflection;

using Composite;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    public class MethodBasedDataSourceAttribute : DataSourceAttribute
    {
        private readonly string _methodName;
        private readonly Type _declaringType;

        public MethodBasedDataSourceAttribute(Type declaringType, string methodName)
        {
            _declaringType = declaringType;
            _methodName = methodName;
        }

        public override object GetData()
        {
            var method = _declaringType.GetMethod(_methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            Verify.IsNotNull(method, "Failed to find method '{0}' on type '{1}'", _methodName, _declaringType.FullName);

            return method.Invoke(null, null);
        }
    }
}
