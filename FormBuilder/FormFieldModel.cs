using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using Composite;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Validation;
using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder
{
    [DebuggerDisplay("{Name}")]
    public class FormFieldModel
    {
        private static readonly IDictionary<Type, InputElementTypeAttribute> DefaultElementType;

        public FormModel OwningForm { get; }

        public string Name { get; set; }
        public bool IsReadOnly { get; set; }
        public IList<Attribute> Attributes { get; }
        public Type ValueType { get; set; }

        public string Id => (OwningForm.Name + "." + Name).Replace(".", "$");
        public bool IsHiddenField => Attributes.OfType<HiddenFieldAttribute>().Any();
        public IEnumerable<FormDependencyAttribute> DependencyAttributes => Attributes.OfType<FormDependencyAttribute>();

        public string Label
        {
            get
            {
                var attr = Attributes.OfType<FieldLabelAttribute>().SingleOrDefault();

                return attr == null ? null : Localization.EvaluateT(this, "Label", attr.Label);
            }
        }

        public string PlaceholderText
        {
            get
            {
                var attr = Attributes.OfType<PlaceholderTextAttribute>().SingleOrDefault();

                return attr == null ? null : Localization.EvaluateT(this, "PlaceholderText", attr.Text);
            }
        }

        public string Help
        {
            get
            {
                var attr = Attributes.OfType<FieldHelpAttribute>().SingleOrDefault();

                return attr == null ? null : Localization.EvaluateT(this, "Help", attr.Help);
            }
        }

        public InputElementTypeAttribute InputElementType
        {
            get
            {
                var inputTypeAttribute = Attributes.OfType<InputElementTypeAttribute>().FirstOrDefault();

                return inputTypeAttribute ?? GetDefaultInputType();
            }
        }

        public IEnumerable<KeyValuePair<string, string>> DataSource
        {
            get
            {
                var datasourceAttribute = Attributes.OfType<DataSourceAttribute>().FirstOrDefault();
                if (datasourceAttribute == null)
                {
                    return null;
                }

                var ds = datasourceAttribute.GetData();
                if (ds == null)
                {
                    return null;
                }

                if (ds is IEnumerable<KeyValuePair<string, string>> listOfKeyValuePair)
                {
                    return listOfKeyValuePair.Select(f => new KeyValuePair<string, string>(f.Key, Localization.Localize(f.Value)));
                }

                if (ds is IDictionary<string, string> dictionary)
                {
                    return dictionary.Select(f => new KeyValuePair<string, string>(f.Key, Localization.Localize(f.Value)));
                }

                if (ds is IEnumerable<string> list)
                {
                    return list.Select(Localization.Localize).Select(str => new KeyValuePair<string, string>(str, str));
                }

                throw new InvalidOperationException("Unsupported data source type: " + ds.GetType().FullName);
            }
        }

        public IEnumerable<FormValidationAttribute> ValidationAttributes
        {
            get
            {
                var required = Attributes.OfType<RequiredFieldAttribute>();
                var theRest = Attributes.OfType<FormValidationAttribute>().Where(a => !(a is RequiredFieldAttribute));

                return required.Concat(theRest).Distinct();
            }
        }

        static FormFieldModel()
        {
            DefaultElementType = new Dictionary<Type, InputElementTypeAttribute>()
            {
                { typeof(bool), new CheckboxInputElementAttribute() },
                { typeof(IEnumerable<string>), new CheckboxInputElementAttribute() },

                { typeof(FormFile), new FileuploadInputElementAttribute() },
                { typeof(IEnumerable<FormFile>), new FileuploadInputElementAttribute() }
            };
        }

        public FormFieldModel(FormModel owningForm, string name, Type valueType, IEnumerable<Attribute> attributes)
        {
            Verify.That(IsValidName(name), "Invalid field name, only a-z and 0-9 is allowed");

            OwningForm = owningForm;
            Name = name;
            Attributes = attributes.ToList();
            ValueType = valueType;
        }

        public bool IsRequired(FormField field)
        {
            var requiredAttribute = Attributes.OfType<RequiredFieldAttribute>().FirstOrDefault();

            return requiredAttribute != null && requiredAttribute.IsRequired(field);
        }

        public void EnsureValueType()
        {
            var type = InputElementType.ResolveValueType(this);
            if (type != null)
            {
                ValueType = type;
            }
        }

        public static bool IsValidName(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z0-9]+$");
        }

        private InputElementTypeAttribute GetDefaultInputType()
        {
            return DefaultElementType.ContainsKey(ValueType) ? DefaultElementType[ValueType] : new TextboxInputElementAttribute();
        }
    }
}