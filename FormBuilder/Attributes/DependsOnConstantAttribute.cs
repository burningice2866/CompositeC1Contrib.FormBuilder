using System;
using System.Linq;

using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder.Attributes
{
    /// <summary>
    /// Use this attribute to register the field as conditional, depending on a value being set on another field.
    /// You can specify multiple dependencies - just one need to me met for your field to be shown.
    /// Fields are not validated if their "depends on" requirement is not met.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DependsOnConstantAttribute : FormDependencyAttribute
    {
        private object[] _requiredValues;

        public DependsOnConstantAttribute(string readFromFieldName, object requiredValue)
            : base(readFromFieldName)
        {
            _requiredValues = new[] { requiredValue };
        }

        public DependsOnConstantAttribute(string readFromFieldName, params object[] requiredValue)
            : base(readFromFieldName)
        {
            _requiredValues = requiredValue;
        }

        public override bool DependencyMet(FormModel model)
        {
            var actualValue = model.Fields.Single(f => f.Name == ReadFromFieldName).Value; 

            foreach (var obj in _requiredValues)
            {
                if (obj.Equals(actualValue))
                {
                    return true;
                }
            }

            return false;
        }

        public override string[] RequiredFieldValues()
        {
            return _requiredValues.Select(s => s.ToString()).ToArray();
        }
    }
}
