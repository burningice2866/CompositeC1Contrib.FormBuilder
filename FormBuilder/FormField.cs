﻿using System;
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
        private static readonly IDictionary<Type, IInputElementHandler> DefaultElementType;

        public FormModel OwningForm { get; private set; }

        public string Name { get; set; }
        public IList<Attribute> Attributes { get; private set; }
        public object Value { get; set; }
        public Type ValueType { get; set; }

        public string Id
        {
            get { return (OwningForm.Name +"." + Name).Replace(".", "$"); }
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
                    return list.Select(FormRenderer.GetLocalized).Select(str => new KeyValuePair<string, string>(str, str));
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

        static FormField()
        {
            DefaultElementType = new Dictionary<Type, IInputElementHandler>() 
            {
                { typeof(bool), new CheckboxInputElement() },
                { typeof(IEnumerable<string>), new CheckboxInputElement() },

                { typeof(FormFile), new FileuploadInputElement() },
                { typeof(IEnumerable<FormFile>), new FileuploadInputElement() },
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

        private IInputElementHandler GetDefaultInputType()
        {
            return DefaultElementType.ContainsKey(ValueType) ? DefaultElementType[ValueType] : new TextboxInputElement();
        }
    }
}