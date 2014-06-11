using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using Composite.Core.Xml;

using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder
{
    public class FormWizard : IFormModel
    {
        public string Name { get; set; }
        public bool ForceHttpSConnection { get; set; }
        public XhtmlDocument IntroText { get; set; }
        public XhtmlDocument SuccessResponse { get; set; }
        public IList<FormWizardStep> Steps { get; private set; }

        public IList<FormValidationRule> ValidationResult { get; private set; }

        public IList<FormField> Fields
        {
            get { return Steps.Select(s => s.FormModel).SelectMany(m => m.Fields).ToList(); }
        }

        public bool HasFileUpload
        {
            get
            {
                return Fields.Any(f => f.ValueType == typeof(FormFile) || f.ValueType == typeof(IEnumerable<FormFile>));
            }
        }

        public FormWizard()
        {
            IntroText = new XhtmlDocument();
            SuccessResponse = new XhtmlDocument();
            Steps = new List<FormWizardStep>();

            ValidationResult = new List<FormValidationRule>();
        }

        public bool ForceHttps
        {
            get { return Steps.Select(s => s.FormModel).Any(m => m.ForceHttps); }
        }

        public virtual void Submit() { }

        public void SetDefaultValues()
        {
            foreach (var model in Steps.Select(s => s.FormModel))
            {
                model.SetDefaultValues();
            }
        }

        public void MapValues(NameValueCollection values, IEnumerable<FormFile> files)
        {
            for (int i = 0; i < Steps.Count; i++)
            {
                var step = Steps[i];
                var model = step.FormModel;
                var stepPrepend = "step-" + (i + 1) + "-";

                var localValues = MapLocal(values, stepPrepend);
                var localFiles = MapLocal(files, stepPrepend);

                model.MapValues(localValues, localFiles);
                model.Validate();
            }
        }

        public void Validate()
        {
            foreach (var model in Steps.Select(s => s.FormModel))
            {
                model.Validate();

                foreach (var result in model.ValidationResult)
                {
                    ValidationResult.Add(result);
                }
            }
        }

        private static NameValueCollection MapLocal(NameValueCollection nvc, string step)
        {
            var local = new NameValueCollection();

            var valueKeys = nvc.AllKeys.Where(k => k.StartsWith(step)).ToArray();
            foreach (var key in valueKeys)
            {
                var name = key.Remove(0, step.Length);
                var value = nvc[key];

                local.Add(name, value);
            }

            return local;
        }

        private static IEnumerable<FormFile> MapLocal(IEnumerable<FormFile> files, string step)
        {
            var newFiles = new List<FormFile>();
            var formFiles = files as IList<FormFile> ?? files.ToList();
            var fileKeys = formFiles.Select(f => f.Key).Where(k => k.StartsWith(step)).ToArray();

            foreach (var key in fileKeys)
            {
                var name = key.Remove(0, step.Length);
                var value = formFiles.Single(f => f.Key == key);

                newFiles.Add(new FormFile
                {
                    Key = name,
                    ContentLength = value.ContentLength,
                    ContentType = value.ContentType,
                    FileName = value.FileName,
                    InputStream = value.InputStream
                });
            }

            return newFiles;
        }
    }
}
