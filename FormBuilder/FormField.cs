using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Validation;
using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder
{
    public class FormField
    {
        public FormModel OwningForm { get; private set; }

        public string Name { get; set; }
        public IEnumerable<Attribute> Attributes { get; private set; }
        public object Value { get; set; }
        public Type ValueType { get; private set; }

        public string Id
        {
            get { return (OwningForm.Name +"." + Name).Replace(".", "_"); }
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
                if (placeholderAttr != null)
                {
                    return placeholderAttr.Text;
                    
                }

                return Label.Label;
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

                var resourceObj = HttpContext.GetGlobalResourceObject("FormBuilderHelp", Id);
                if (resourceObj != null)
                {
                    return (string)resourceObj;
                }

                return String.Empty;
            }
        }

        public IInputElementHandler InputTypeHandler
        {
            get
            {
                var inputTypeAttribute = Attributes.OfType<InputElementProviderAttribute>().FirstOrDefault();
                if (inputTypeAttribute != null)
                {
                    return inputTypeAttribute.GetInputFieldTypeHandler();
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
                    return list.Select(str => FormRenderer.GetLocalized(str)).Select(str => new KeyValuePair<string, string>(str, str));
                }

                throw new InvalidOperationException("Unsupported data source type: " + ds.GetType().FullName);
            }
        }

        public IEnumerable<FormValidationAttribute> ValidationAttributes
        {
            get { return Attributes.OfType<FormValidationAttribute>(); }
        }

        public IEnumerable<FormDependencyAttribute> DependencyAttributes
        {
            get { return Attributes.OfType<FormDependencyAttribute>(); }
        }

        public FormField(FormModel owningForm, string name, Type valueType, IList<Attribute> attributes)
        {
            OwningForm = owningForm;
            Name = name;
            Attributes = attributes;
            ValueType = valueType;
        }

        private IInputElementHandler GetDefaultInputType()
        {
            if (ValueType == typeof(bool)) return new CheckboxInputElement();
            if (ValueType == typeof(FormFile) || ValueType == typeof(IEnumerable<FormFile>)) return new FileuploadInputElement();

            return new TextboxInputElement();
        }
    }
}
