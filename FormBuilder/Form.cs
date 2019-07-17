using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Web;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Data;
using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder
{
    [DebuggerDisplay("{Name}")]
    public sealed class Form : IModelInstance
    {
        private static readonly IDictionary<Type, IValueMapper> ValueMappers;

        private IDictionary<FormField, ValidationResultList> _ruleList;

        public FormModel Model { get; private set; }
        public IList<FormField> Fields { get; private set; }

        public NameValueCollection SubmittedValues { get; private set; }
        public IDictionary<string, object> FormData { get; private set; }

        public string Name => Model.Name;

        public IList<Attribute> Attributes => Model.Attributes;

        public bool DisableAntiForgery => Model.DisableAntiForgery;

        public bool RequiresCaptcha => Model.RequiresCaptcha;

        public bool ForceHttps => Model.ForceHttps;

        public bool HasFileUpload => Model.HasFileUpload;

        public string SubmitButtonLabel => Model.SubmitButtonLabel;

        static Form()
        {
            ValueMappers = CompositionContainerFacade.GetExportedValues<IValueMapper>().ToDictionary(m => m.ValueMapperFor);
        }

        public Form(FormModel model)
        {
            Model = model;
            FormData = new Dictionary<string, object>();
            Fields = model.Fields.Select(f => new FormField(f, this)).ToList().AsReadOnly();

            model.Constructor?.Invoke(this);
        }

        public bool IsValid(IEnumerable<string> fieldNames)
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

        public bool IsDependencyMetRecursive(FormField field)
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

        public void SetDefaultValues()
        {
            Model?.SetDefaultValuesHandler(this);
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

        public ValidationResultList Validate(ValidationOptions options)
        {
            var validationList = new ValidationResultList();

            if (SubmittedValues == null || SubmittedValues.AllKeys.Length == 0)
            {
                return validationList;
            }

            if (Model.OnValidateHandler != null)
            {
                var e = new FormValidationEventArgs(SubmittedValues);

                Model.OnValidateHandler(this, e);

                if (e.Cancel)
                {
                    return validationList;
                }
            }

            EnsureRulesList();

            foreach (var kvp in _ruleList)
            {
                if (!options.ValidateFiles)
                {
                    var valueType = kvp.Key.ValueType;

                    if (valueType == typeof(FormFile) || valueType == typeof(IEnumerable<FormFile>))
                    {
                        continue;
                    }
                }

                var result = ValidationResultList.GetFormValidationResult(kvp.Value, false);

                validationList.AddRange(result);
            }

            if (options.ValidateCaptcha)
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

                if (!IsDependencyMetRecursive(field))
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

        private void MapValueToField(FormField field, string val)
        {
            if (ValueMappers.TryGetValue(field.ValueType, out var mapper))
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
