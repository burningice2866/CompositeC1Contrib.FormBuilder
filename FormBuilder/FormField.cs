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
    public class FormField
    {
        private static readonly IDictionary<Type, InputElementTypeAttribute> DefaultElementType;

        public FormModel OwningForm { get; private set; }

        public string Name { get; set; }
        public bool IsReadOnly { get; set; }
        public IList<Attribute> Attributes { get; private set; }
        public object Value { get; set; }
        public Type ValueType { get; set; }

        public string Id
        {
            get { return (OwningForm.Name + "." + Name).Replace(".", "$"); }
        }

        public FieldLabelAttribute Label
        {
            get { return Attributes.OfType<FieldLabelAttribute>().SingleOrDefault(); }
        }

        public string PlaceholderText
        {
            get
            {
                var placeholderAttr = Attributes.OfType<PlaceholderTextAttribute>().SingleOrDefault();

                return placeholderAttr != null ? placeholderAttr.Text : String.Empty;
            }
        }

        public string Help
        {
            get
            {
                var helpAttribute = Attributes.OfType<FieldHelpAttribute>().FirstOrDefault();
                if (helpAttribute != null)
                {
                    return helpAttribute.Help;
                }

                var resourceObj = HttpContext.GetGlobalResourceObject("FormBuilderHelp", Id.Replace("$", "_"));
                if (resourceObj != null)
                {
                    return (string)resourceObj;
                }

                return String.Empty;
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
            get { return Attributes.Any(a => a is RequiredFieldAttribute); }
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

                var dict = ds as IDictionary<string, string>;
                if (dict != null)
                {
                    return dict.Select(f => new KeyValuePair<string, string>(f.Key, FormRenderer.GetLocalized(f.Value)));
                }

                var list = ds as IEnumerable<string>;
                if (list != null)
                {
                    return list.Select(FormRenderer.GetLocalized).Select(str => new KeyValuePair<string, string>(str, str));
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

        static FormField()
        {
            DefaultElementType = new Dictionary<Type, InputElementTypeAttribute>() 
            {
                { typeof(bool), new CheckboxInputElementAttribute() },
                { typeof(IEnumerable<string>), new CheckboxInputElementAttribute() },

                { typeof(FormFile), new FileuploadInputElementAttribute() },
                { typeof(IEnumerable<FormFile>), new FileuploadInputElementAttribute() }
            };
        }

        public FormField(FormModel owningForm, string name, Type valueType, IList<Attribute> attributes)
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

        private InputElementTypeAttribute GetDefaultInputType()
        {
            return DefaultElementType.ContainsKey(ValueType) ? DefaultElementType[ValueType] : new TextboxInputElementAttribute();
        }

        public void EnsureValueType()
        {
            if (InputElementType is CheckboxInputElementAttribute)
            {
                ValueType = typeof(bool);
            }

            if (InputElementType is CheckboxInputElementAttribute && DataSource != null)
            {
                ValueType = typeof(IEnumerable<string>);
            }

            if (InputElementType is FileuploadInputElementAttribute)
            {
                ValueType = typeof(FormFile);
            }
        }
    }
}