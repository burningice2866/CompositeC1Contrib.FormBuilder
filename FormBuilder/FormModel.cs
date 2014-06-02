using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using Composite;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder
{
    public sealed class FormModel : IFormModel
    {
        private static readonly IDictionary<Type, Action<FormField, string>> ValueMappers = new Dictionary<Type, Action<FormField, string>>();
        
        private IDictionary<FormField, IList<FormValidationRule>> _ruleList;

        public NameValueCollection SubmittedValues { get; private set; }

        public string Name { get; private set; }
        public IList<FormField> Fields { get; private set; }
        public IList<FormValidationRule> ValidationResult { get; private set; }
        public IList<Attribute> Attributes { get; private set; }

        public Action<FormValidationEventArgs> OnValidateHandler { get; set; }
        public Action<FormModel> SetDefaultValuesHandler { get; set; }

        public bool ForceHttps
        {
            get
            {
                var forceHttpsAttr = Attributes.OfType<ForceHttpsConnectionAttribute>().SingleOrDefault();

                return forceHttpsAttr != null;
            }
        }

        public bool HasFileUpload
        {
            get
            {
                return Fields.Any(f => f.ValueType == typeof(FormFile) || f.ValueType == typeof(IEnumerable<FormFile>));
            }
        }

        public string SubmitButtonLabel
        {
            get
            {
                var label = "Indsend";

                var labelAttribute = Attributes.OfType<SubmitButtonLabelAttribute>().FirstOrDefault();
                if (labelAttribute != null)
                {
                    label = labelAttribute.Label;
                }

                return label;
            }
        }

        static FormModel()
        {
            ValueMappers.Add(typeof(int), (f, val) =>
            {
                int i;
                int.TryParse(val, out i);

                f.Value = i;
            });

            ValueMappers.Add(typeof(int?), (f, val) =>
            {
                int i;
                if (int.TryParse(val, out i))
                {
                    f.Value = i;
                }
                else
                {
                    f.Value = null;
                }
            });

            ValueMappers.Add(typeof(decimal), (f, val) =>
            {
                decimal d;
                decimal.TryParse(val, out d);

                f.Value = d;
            });

            ValueMappers.Add(typeof(decimal?), (f, val) =>
            {
                decimal d;
                if (decimal.TryParse(val, out d))
                {
                    f.Value = d;
                }
                else
                {
                    f.Value = null;
                }
            });

            ValueMappers.Add(typeof(Guid), (f, val) =>
            {
                Guid g;
                Guid.TryParse(val, out g);

                f.Value = g;
            });

            ValueMappers.Add(typeof(Guid?), (f, val) =>
            {
                Guid g;
                if (Guid.TryParse(val, out g))
                {
                    f.Value = g;
                }
                else
                {
                    f.Value = null;
                }
            });

            ValueMappers.Add(typeof(DateTime), (f, val) =>
            {
                DateTime dt;
                var formatAttr = f.Attributes.OfType<DisplayFormatAttribute>().SingleOrDefault();

                if (formatAttr != null)
                {
                    DateTime.TryParseExact(val, formatAttr.FormatString, CultureInfo.CurrentUICulture, DateTimeStyles.None, out dt);
                }
                else
                {
                    DateTime.TryParse(val, out dt);
                }

                f.Value = dt;
            });

            ValueMappers.Add(typeof(DateTime?), (f, val) =>
            {
                DateTime dt;
                var formatAttr = f.Attributes.OfType<DisplayFormatAttribute>().SingleOrDefault();

                if (formatAttr != null)
                {
                    if (DateTime.TryParseExact(val, formatAttr.FormatString, CultureInfo.CurrentUICulture, DateTimeStyles.None, out dt))
                    {
                        f.Value = dt;
                    }
                    else
                    {
                        f.Value = null;
                    }
                }
                else
                {
                    if (DateTime.TryParse(val, out dt))
                    {
                        f.Value = dt;
                    }
                    else
                    {
                        f.Value = null;
                    }
                }
            });

            ValueMappers.Add(typeof(bool), (f, val) =>
            {
                bool b;

                if (val == "on")
                {
                    b = true;
                }
                else
                {
                    bool.TryParse(val, out b);
                }

                f.Value = b;
            });

            ValueMappers.Add(typeof(string), (f, val) =>
            {
                f.Value = val;
            });

            ValueMappers.Add(typeof(IEnumerable<string>), (f, val) =>
            {
                f.Value = val.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            });
        }

        public FormModel(string name)
        {
            Verify.That(IsValidName(name), "Invalid form name, only a-z and 0-9 is allowed");

            Name = name;

            Fields = new List<FormField>();
            ValidationResult = new List<FormValidationRule>();
            Attributes = new List<Attribute>();
        }

        public bool IsValid(string[] fieldNames)
        {
            EnsureRulesList();

            foreach (var fieldName in fieldNames)
            {
                var field = Fields.Single(f => f.Name == fieldName);
                var result = GetFormValidationResult(_ruleList[field], true);

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
                var e = new FormValidationEventArgs(SubmittedValues);

                OnValidateHandler(e);

                if (e.Cancel)
                {
                    return;
                }
            }

            EnsureRulesList();

            var validationList = new List<FormValidationRule>();
            foreach (var list in _ruleList.Values)
            {
                var result = GetFormValidationResult(list, false);

                validationList.AddRange(result);
            }

            var requiresCaptchaAttr = Attributes.OfType<RequiresCaptchaAttribute>().SingleOrDefault();
            if (requiresCaptchaAttr != null)
            {
                var form = HttpContext.Current.Request.Form;
                var encrypted = form[RequiresCaptchaAttribute.HiddenFieldName];
                var postedValue = form[RequiresCaptchaAttribute.InputName];

                var isValid = RequiresCaptchaAttribute.IsValid(encrypted, postedValue);
                if (!isValid)
                {
                    validationList.Add(new FormValidationRule(new[] { RequiresCaptchaAttribute.InputName }, Localization.Captcha_Error));
                }
            }

            ValidationResult = validationList;
        }

        public void SetDefaultValues()
        {
            if (SetDefaultValuesHandler != null)
            {
                SetDefaultValuesHandler(this);
            }
        }

        public void MapValues(NameValueCollection values, IEnumerable<FormFile> files)
        {
            SubmittedValues = values;
            if (SubmittedValues == null)
            {
                return;
            }

            foreach (var field in Fields.Where(f => !f.IsReadOnly))
            {
                var val = (values[field.Name] ?? String.Empty).Trim();

                MapValueToField(field, val);
                MapFilesToField(field, files);
            }
        }

        public static bool IsValidName(string name)
        {
            var parts = name.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            return parts.Length >= 2 && parts.All(p => Regex.IsMatch(p, @"^[a-zA-Z0-9]+$"));
        }

        private static IEnumerable<FormValidationRule> GetFormValidationResult(IEnumerable<FormValidationRule> rules, bool skipMultipleFieldsRules)
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

        private void EnsureRulesList()
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
                if (!validateField)
                {
                    continue;
                }

                foreach (var attr in attributes)
                {
                    var validationAttribute = attr;
                    if (validationAttribute == null)
                    {
                        continue;
                    }

                    var rule = validationAttribute.CreateRule(field);

                    list.Add(rule);
                }
            }
        }

        private bool IsDependencyMetRecursive(FormField field)
        {
            var attributes = field.DependencyAttributes.ToList();
            if (!attributes.Any())
            {
                return true;
            }

            foreach (var dependencyAttribute in attributes)
            {
                var isValid = dependencyAttribute.DependencyMet(this);
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

            return true;
        }

        private static void MapValueToField(FormField field, string val)
        {
            Action<FormField, string> mapper;

            if (ValueMappers.TryGetValue(field.ValueType, out mapper))
            {
                mapper(field, val);
            }
        }

        private static void MapFilesToField(FormField field, IEnumerable<FormFile> files)
        {
            if (files == null)
            {
                return;
            }

            var fieldFiles = files.Where(f => f.Key == field.Name).ToList();
            if (!fieldFiles.Any())
            {
                return;
            }

            if (field.ValueType == typeof(FormFile))
            {
                field.Value = fieldFiles.First();
            }
            else if (field.ValueType == typeof(IEnumerable<FormFile>))
            {
                field.Value = fieldFiles;
            }
        }
    }
}
