using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using Composite;

using CompositeC1Contrib.FormBuilder.Attributes;

namespace CompositeC1Contrib.FormBuilder
{
    [DebuggerDisplay("{Name}")]
    public sealed class FormModel : IModel
    {
        public string Name { get; }
        public IList<FormFieldModel> Fields { get; }
        public IList<Attribute> Attributes { get; }

        public Action<Form> Constructor { get; set; }
        public Action<Form, FormValidationEventArgs> OnValidateHandler { get; set; }
        public Action<Form> SetDefaultValuesHandler { get; set; }

        public bool DisableAntiForgery => Attributes.OfType<DisableAntiForgeryAttribute>().Any();

        public bool RequiresCaptcha => Attributes.OfType<RequiresCaptchaAttribute>().Any();

        public bool ForceHttps => Attributes.OfType<ForceHttpsConnectionAttribute>().Any();

        public bool HasFileUpload => Fields.Any(f => f.ValueType == typeof(FormFile) || f.ValueType == typeof(IEnumerable<FormFile>));

        public string SubmitButtonLabel
        {
            get
            {
                var attr = Attributes.OfType<SubmitButtonLabelAttribute>().SingleOrDefault();

                return Localization.EvaluateT(this, "SubmitButtonLabel", attr == null ? "Indsend" : attr.Label);
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
