using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using Composite;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Data;
using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder
{
    public sealed class FormModel : IFormModel
    {
        private static readonly IDictionary<Type, IValueMapper> ValueMappers;

        private IDictionary<FormField, ValidationResultList> _ruleList;

        public NameValueCollection SubmittedValues { get; private set; }

        public string Name { get; private set; }
        public IList<FormField> Fields { get; private set; }
        public IList<Attribute> Attributes { get; private set; }

        public Action<FormValidationEventArgs> OnValidateHandler { get; set; }
        public Action<FormModel> SetDefaultValuesHandler { get; set; }

        public bool DisableAntiForgery
        {
            get { return Attributes.OfType<DisableAntiForgeryAttribute>().Any(); }
        }

        public bool RequiresCaptcha
        {
            get { return Attributes.OfType<RequiresCaptchaAttribute>().Any(); }
        }

        public bool ForceHttps
        {
            get { return Attributes.OfType<ForceHttpsConnectionAttribute>().Any(); }
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
            ValueMappers = CompositionContainerFacade.GetExportedValues<IValueMapper>().ToDictionary(m => m.ValueMapperFor);
        }

        public FormModel(string name)
        {
            Verify.That(IsValidName(name), "Invalid form name, only a-z and 0-9 is allowed");

            Name = name;

            Fields = new List<FormField>();
            Attributes = new List<Attribute>();
        }

        public bool IsValid(string[] fieldNames)
        {
            EnsureRulesList();

            foreach (var fieldName in fieldNames)
            {
                var field = Fields.Single(f => f.Name == fieldName);
                var result = ValidationResultList.GetFormValidationResult(_ruleList[field], true);

                if (result.Any(r => r.AffectedFormIds.Contains(fieldName)))
                {
                    return false;
                }
            }

            return true;
        }

        public ValidationResultList Validate(bool validateCaptcha)
        {
            var validationList = new ValidationResultList();

            if (SubmittedValues == null || SubmittedValues.AllKeys.Length == 0)
            {
                return validationList;
            }

            if (OnValidateHandler != null)
            {
                var e = new FormValidationEventArgs(SubmittedValues);

                OnValidateHandler(e);

                if (e.Cancel)
                {
                    return validationList;
                }
            }

            EnsureRulesList();

            foreach (var list in _ruleList.Values)
            {
                var result = ValidationResultList.GetFormValidationResult(list, false);

                validationList.AddRange(result);
            }

            if (validateCaptcha)
            {
                var requiresCaptchaAttr = Attributes.OfType<RequiresCaptchaAttribute>().SingleOrDefault();
                if (requiresCaptchaAttr != null)
                {
                    var form = new HttpContextWrapper(HttpContext.Current);

                    requiresCaptchaAttr.Validate(form, validationList);
                }
            }

            return validationList;
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
                var val = values[field.Name];
                if (val != null)
                {
                    val = val.Trim();

                    MapValueToField(field, val);
                }

                MapFilesToField(field, files);
            }
        }

        public static bool IsValidName(string name)
        {
            var parts = name.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            return parts.Length >= 2 && parts.All(p => Regex.IsMatch(p, @"^[a-zA-Z0-9]+$"));
        }

        private void EnsureRulesList()
        {
            _ruleList = new Dictionary<FormField, ValidationResultList>();

            foreach (var field in Fields)
            {
                if (!_ruleList.ContainsKey(field))
                {
                    _ruleList.Add(field, new ValidationResultList());
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
            IValueMapper mapper;
            if (ValueMappers.TryGetValue(field.ValueType, out mapper))
            {
                mapper.MapValue(field, val);
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
