using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using Composite;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Validation;
using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder
{
    public class FormFieldModel
    {
        private static readonly IDictionary<Type, InputElementTypeAttribute> DefaultElementType;

        public FormModel OwningForm { get; private set; }

        public string Name { get; set; }
        public bool IsReadOnly { get; set; }
        public IList<Attribute> Attributes { get; private set; }
        public Type ValueType { get; set; }

        public string Id
        {
            get { return (OwningForm.Name + "." + Name).Replace(".", "$"); }
        }

        public string Label
        {
            get
            {
                var attr = Attributes.OfType<FieldLabelAttribute>().SingleOrDefault();
                if (attr == null)
                {
                    return null;
                }

                var label = attr.Label;
                if (label == null)
                {
                    label = ResolveLocalized("Label");
                }

                return label;
            }
        }

        public string PlaceholderText
        {
            get
            {
                var attr = Attributes.OfType<PlaceholderTextAttribute>().SingleOrDefault();
                if (attr == null)
                {
                    return null;
                }

                var text = attr.Text;
                if (text == null)
                {
                    text = ResolveLocalized("PlaceholderText");
                }

                return text;
            }
        }

        public string Help
        {
            get
            {
                var attr = Attributes.OfType<FieldHelpAttribute>().SingleOrDefault();
                if (attr == null)
                {
                    return null;
                }

                var help = attr.Help;
                if (help == null)
                {
                    help = ResolveLocalized("Help");
                }

                return help;
            }
        }

        public InputElementTypeAttribute InputElementType
        {
            get
            {
                var inputTypeAttribute = Attributes.OfType<InputElementTypeAttribute>().FirstOrDefault();
                if (inputTypeAttribute != null)
                {
                    return inputTypeAttribute;
                }

                return GetDefaultInputType();
            }
        }

        public bool IsRequired
        {
            get
            {
                var requiredAttribute = Attributes.OfType<RequiredFieldAttribute>().FirstOrDefault();

                return requiredAttribute != null && requiredAttribute.IsRequired;
            }
        }

        public bool IsHiddenField
        {
            get { return Attributes.OfType<HiddenFieldAttribute>().Any(); }
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

                var listOfKeyValuePair = ds as IEnumerable<KeyValuePair<string, string>>;
                if (listOfKeyValuePair != null)
                {
                    return listOfKeyValuePair.Select(f => new KeyValuePair<string, string>(f.Key, Strings.GetLocalized(f.Value)));
                }

                var dictionary = ds as IDictionary<string, string>;
                if (dictionary != null)
                {
                    return dictionary.Select(f => new KeyValuePair<string, string>(f.Key, Strings.GetLocalized(f.Value)));
                }

                var list = ds as IEnumerable<string>;
                if (list != null)
                {
                    return list.Select(Strings.GetLocalized).Select(str => new KeyValuePair<string, string>(str, str));
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

        public IEnumerable<FormDependencyAttribute> DependencyAttributes
        {
            get { return Attributes.OfType<FormDependencyAttribute>(); }
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

        public FormFieldModel(FormModel owningForm, string name, Type valueType, IList<Attribute> attributes)
        {
            Verify.That(IsValidName(name), "Invalid field name, only a-z and 0-9 is allowed");

            OwningForm = owningForm;
            Name = name;
            Attributes = attributes;
            ValueType = valueType;
        }

        public static bool IsValidName(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z0-9]+$");
        }

        public void EnsureValueType()
        {
            var type = InputElementType.ResolveValueType(this);
            if (type != null)
            {
                ValueType = type;
            }
        }

        private InputElementTypeAttribute GetDefaultInputType()
        {
            return DefaultElementType.ContainsKey(ValueType) ? DefaultElementType[ValueType] : new TextboxInputElementAttribute();
        }

        private string ResolveLocalized(string type)
        {
            var key = "Forms." + OwningForm.Name + "." + Name + "." + type;

            var value = HttpContext.GetGlobalResourceObject("FormBuilder", key) as string;
            if (value == null)
            {
                return key;
            }

            return value;
        }
    }
}