using System;
using System.Collections.Generic;
using System.Linq;

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
        public IList<object> RequiredFieldValues { get; private set; }

        public DependsOnConstantAttribute(string readFromFieldName, params object[] requiredFieldValues)
            : base(readFromFieldName)
        {
            RequiredFieldValues = requiredFieldValues.ToList();
        }

        public override bool DependencyMet(Form form)
        {
            var field = form.Fields.Get(ReadFromFieldName);
            var actualValue = field.Value;

            foreach (var obj in RequiredFieldValues)
            {
                var actualValueList = actualValue as IEnumerable<string>;
                if (actualValueList != null)
                {
                    return actualValueList.Any(f => obj.Equals(f));
                }

                if (obj.Equals(actualValue))
                {
                    return true;
                }
            }

            return false;
        }

        public override object[] ResolveRequiredFieldValues()
        {
            return RequiredFieldValues.ToArray();
        }
    }
}
