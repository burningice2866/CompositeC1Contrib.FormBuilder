using System;
using System.Collections.Generic;
using System.Diagnostics;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Validation;
using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder
{
    [DebuggerDisplay("{Name}")]
    public class FormField
    {
        public FormFieldModel Model { get; private set; }

        public Form OwningForm { get; private set; }
        public object Value { get; set; }

        public string Name
        {
            get { return Model.Name; }
        }

        public bool IsReadOnly
        {
            get { return Model.IsReadOnly; }
        }

        public IList<Attribute> Attributes
        {
            get { return Model.Attributes; }
        }

        public Type ValueType
        {
            get { return Model.ValueType; }
        }

        public string Id
        {
            get { return Model.Id; }
        }

        public string Label
        {
            get { return Model.Label; }
        }

        public string PlaceholderText
        {
            get { return Model.PlaceholderText; }
        }

        public string Help
        {
            get { return Model.Help; }
        }

        public InputElementTypeAttribute InputElementType
        {
            get { return Model.InputElementType; }
        }

        public bool IsRequired
        {
            get { return Model.IsRequired(this); }
        }

        public bool IsHiddenField
        {
            get { return Model.IsHiddenField; }
        }

        public IEnumerable<KeyValuePair<string, string>> DataSource
        {
            get { return Model.DataSource; }
        }

        public IEnumerable<FormValidationAttribute> ValidationAttributes
        {
            get { return Model.ValidationAttributes; }
        }

        public IEnumerable<FormDependencyAttribute> DependencyAttributes
        {
            get { return Model.DependencyAttributes; }
        }

        public FormField(FormFieldModel model, Form instance)
        {
            Model = model;
            OwningForm = instance;
        }

        public bool IsDependencyMetRecursive()
        {
            return OwningForm.IsDependencyMetRecursive(this);
        }

        public string GetValueAsString()
        {
            if (Value == null)
            {
                return String.Empty;
            }

            if (ValueType == typeof(DateTime))
            {
                return ((DateTime)Value).ToString("yyyy-MM-dd");
            }

            if (ValueType == typeof(DateTime?))
            {
                var dt = (DateTime?)Value;

                return dt.Value.ToString("yyyy-MM-dd");
            }

            return Value.ToString();
        }
    }
}