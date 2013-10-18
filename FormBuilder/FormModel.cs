using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Validation;
using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder
{
    public class FormModel
    {
        private IDictionary<FormField, IList<FormValidationRule>> _ruleList = null;

        public NameValueCollection SubmittedValues { get; private set; }

        public string Name { get; private set; }
        public IList<FormField> Fields { get; private set; }
        public IList<FormValidationRule> ValidationResult { get; private set; }
        public IList<Attribute> Attributes { get; private set; }
        public FormOptions Options { get; set; }

        public Action OnSubmitHandler { get; set; }
        public Action<FormValidationEventArgs> OnValidateHandler { get; set; }

        public bool ForceHttps
        {
            get
            {
                var forceHttpsAttr = Attributes.OfType<ForceHttpsConnectionAttribute>().SingleOrDefault();
                if (forceHttpsAttr != null)
                {
                    return forceHttpsAttr.ForceHttps;
                }

                return false;
            }
        }

        public bool HasFileUpload
        {
            get
            {
                return Fields.Any(f => f.ValueType == typeof(FormFile) || f.ValueType == typeof(IEnumerable<FormFile>));
            }
        }

        public FormModel(string name)
        {
            Name = name;

            Fields = new List<FormField>();
            ValidationResult = new List<FormValidationRule>();
            Attributes = new List<Attribute>();
        }

        public bool IsValid(string[] fieldNames)
        {
            ensureRulesList();

            foreach (var fieldName in fieldNames)
            {
                var field = Fields.Single(f => f.Name == fieldName);
                var result = getFormValidationResult(_ruleList[field], true);

                if (result.Any(r => r.AffectedFormIds.Contains(fieldName)))
                {
                    return false;
                }
            }

            return true;
        }

        public void Validate()
        {
            if (SubmittedValues == null || SubmittedValues.AllKeys.Length == 0)
            {
                return;
            }

            if (OnValidateHandler != null)
            {
                var e = new FormValidationEventArgs();

                OnValidateHandler(e);

                if (e.Cancel)
                {
                    return;
                }
            }

            ensureRulesList();

            var validationList = new List<FormValidationRule>();
            foreach (var list in _ruleList.Values)
            {
                var result = getFormValidationResult(list, false);

                validationList.AddRange(result);
            }

            ValidationResult = validationList;
        }

        public void MapValues(NameValueCollection values, IEnumerable<FormFile> files)
        {
            SubmittedValues = values;

            if (values != null)
            {
                foreach (var field in Fields)
                {
                    var val = (values[field.Name] ?? String.Empty).Trim();

                    MapValueToField(field, val);
                    MapFilesToField(field, files);
                }
            }
        }

        public static FormModel GetCurrent(string name)
        {
            return (FormModel)HttpContext.Current.Items["__FormModel__" + name];
        }

        public static void SetCurrent(string name, FormModel value)
        {
            HttpContext.Current.Items["__FormModel__" + name] = value;
        }

        private IList<FormValidationRule> getFormValidationResult(IList<FormValidationRule> rules, bool skipMultipleFieldsRules)
        {
            return rules.Where(r =>
            {
                if (skipMultipleFieldsRules)
                {
                    return r.AffectedFormIds.Count() == 1;
                }

                return true;
            })
            .Where(r => !r.Rule())
            .ToList();
        }

        private void ensureRulesList()
        {
            _ruleList = new Dictionary<FormField, IList<FormValidationRule>>();

            foreach (var field in Fields)
            {
                if (!_ruleList.ContainsKey(field))
                {
                    _ruleList.Add(field, new List<FormValidationRule>());
                }

                var list = _ruleList[field];
                var attributes = field.ValidationAttributes;

                var validateField = IsDependencyMetRecursive(field);
                if (validateField)
                {
                    foreach (var attr in attributes)
                    {
                        var validationAttribute = attr as FormValidationAttribute;
                        if (validationAttribute != null)
                        {
                            var rule = validationAttribute.CreateRule(field);

                            list.Add(rule);
                        }
                    }
                }
            }
        }

        private bool IsDependencyMetRecursive(FormField field)
        {
            var isValid = true;

            var attributes = field.DependencyAttributes;
            if (!attributes.Any())
            {
                return true;
            }
            
            foreach (var dependencyAttribute in attributes)
            {
                isValid = dependencyAttribute.DependencyMet(this);

                if (!isValid)
                {
                    return false;
                }

                var dependencyField = Fields.Single(f => f.Name == dependencyAttribute.ReadFromFieldName);
                isValid = IsDependencyMetRecursive(dependencyField);

                if (!isValid)
                {
                    return false;
                }
            }

            return isValid;
        }

        private void MapFilesToField(FormField field, IEnumerable<FormFile> files)
        {
            if (files != null && files.Any())
            {
                files = files.Where(f => f.Key == field.Name);

                if (field.ValueType == typeof(FormFile))
                {
                    field.Value = files.FirstOrDefault();

                }
                else if (field.ValueType == typeof(IEnumerable<FormFile>))
                {
                    field.Value = files;
                }
            }
        }

        private void MapValueToField(FormField field, string val)
        {
            var formatAttr = field.Attributes.OfType<DisplayFormatAttribute>().SingleOrDefault();

            if (field.ValueType == typeof(int))
            {
                var i = 0;
                int.TryParse(val, out i);

                field.Value = i;
            }
            else if (field.ValueType == typeof(int?))
            {
                var i = 0;
                if (int.TryParse(val, out i))
                {
                    field.Value = i;
                }
                else
                {
                    field.Value = null;
                }
            }

            else if (field.ValueType == typeof(decimal))
            {
                var d = 0m;
                decimal.TryParse(val, out d);

                field.Value = d;
            }
            else if (field.ValueType == typeof(decimal?))
            {
                var d = 0m;
                if (decimal.TryParse(val, out d))
                {
                    field.Value = d;
                }
                else
                {
                    field.Value = null;
                }
            }

            else if (field.ValueType == typeof(bool))
            {
                var b = false;

                if (val == "on")
                {
                    b = true;
                }
                else
                {
                    bool.TryParse(val, out b);
                }

                field.Value = b;
            }

            else if (field.ValueType == typeof(string))
            {
                field.Value = val;
            }
            else if (field.ValueType == typeof(IEnumerable<string>))
            {
                field.Value = val.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            else if (field.ValueType == typeof(DateTime))
            {
                var dt = DateTime.Now;

                if (formatAttr != null)
                {
                    DateTime.TryParseExact(val, formatAttr.FormatString, CultureInfo.CurrentUICulture, DateTimeStyles.None, out dt);
                }
                else
                {
                    DateTime.TryParse(val, out dt);
                }

                field.Value = dt;
            }
            else if (field.ValueType == typeof(DateTime?))
            {
                var dt = DateTime.MinValue;
                if (formatAttr != null)
                {
                    if (DateTime.TryParseExact(val, formatAttr.FormatString, CultureInfo.CurrentUICulture, DateTimeStyles.None, out dt))
                    {
                        field.Value = dt;
                    }
                    else
                    {
                        field.Value = null;
                    }
                }
                else
                {
                    if (DateTime.TryParse(val, out dt))
                    {
                        field.Value = dt;
                    }
                    else
                    {
                        field.Value = null;
                    }
                }
            }
        }
    }
}
