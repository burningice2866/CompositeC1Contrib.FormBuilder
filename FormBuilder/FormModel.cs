using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Composite;

using CompositeC1Contrib.FormBuilder.Attributes;

namespace CompositeC1Contrib.FormBuilder
{
    public sealed class FormModel : IModel
    {
        public string Name { get; set; }
        public IList<FormFieldModel> Fields { get; private set; }
        public IList<Attribute> Attributes { get; private set; }

        public Action<Form> Constructor { get; set; }
        public Action<Form, FormValidationEventArgs> OnValidateHandler { get; set; }
        public Action<Form> SetDefaultValuesHandler { get; set; }

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

        public FormModel(string name)
        {
            Verify.That(IsValidName(name), "Invalid form name, only a-z, 0-9 and symbols - is allowed");

            Name = name;

            Fields = new List<FormFieldModel>();
            Attributes = new List<Attribute>();
        }

        public static bool IsValidName(string name)
        {
            var parts = name.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            return parts.Length >= 2 && parts.All(p => Regex.IsMatch(p, @"^[a-zA-Z0-9-]+$"));
        }
    }
}
