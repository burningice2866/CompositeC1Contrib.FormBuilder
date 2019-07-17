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
        public FormFieldModel Model { get; }

        public Form OwningForm { get; }
        public object Value { get; set; }

        public string Name => Model.Name;

        public bool IsReadOnly => Model.IsReadOnly;

        public IList<Attribute> Attributes => Model.Attributes;

        public Type ValueType => Model.ValueType;

        public string Id => Model.Id;

        public string Label => Model.Label;

        public string PlaceholderText => Model.PlaceholderText;

        public string Help => Model.Help;

        public InputElementTypeAttribute InputElementType => Model.InputElementType;

        public bool IsRequired => Model.IsRequired(this);

        public bool IsHiddenField => Model.IsHiddenField;

        public IEnumerable<KeyValuePair<string, string>> DataSource => Model.DataSource;

        public IEnumerable<FormValidationAttribute> ValidationAttributes => Model.ValidationAttributes;

        public IEnumerable<FormDependencyAttribute> DependencyAttributes => Model.DependencyAttributes;

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