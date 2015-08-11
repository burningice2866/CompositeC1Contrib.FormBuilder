using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder
{
    public class Wizard : IModelInstance
    {
        public WizardModel Model { get; private set; }
        public IList<WizardStep> Steps { get; private set; }

        public string Name
        {
            get { return Model.Name; }
        }

        public bool RequiresCaptcha
        {
            get { return Model.RequiresCaptcha; }
        }

        public bool ForceHttps
        {
            get { return Model.ForceHttps; }
        }

        public IList<FormField> Fields
        {
            get { return Steps.Select(s => s.Form).SelectMany(m => m.Fields).ToList(); }
        }

        public bool DisableAntiForgery
        {
            get { return Steps.Select(s => s.Form).Any(m => m.DisableAntiForgery); }
        }

        public bool HasFileUpload
        {
            get { return Fields.Any(f => f.ValueType == typeof(FormFile) || f.ValueType == typeof(IEnumerable<FormFile>)); }
        }

        public Wizard(WizardModel model)
        {
            Model = model;
            Steps = Model.Steps.Select(s => new WizardStep(s)).ToList();
        }

        public void SetDefaultValues()
        {
            foreach (var form in Steps.Select(s => s.Form))
            {
                form.SetDefaultValues();
            }
        }

        public void MapValues(NameValueCollection values, IEnumerable<FormFile> files)
        {
            for (int i = 0; i < Steps.Count; i++)
            {
                var step = Steps[i];
                var form = step.Form;
                var stepPrepend = "step-" + (i + 1) + "-";

                var localValues = MapLocal(values, stepPrepend);
                var localFiles = MapLocal(files, stepPrepend);

                form.MapValues(localValues, localFiles);
            }
        }

        public ValidationResultList Validate(ValidationOptions options)
        {
            var list = new ValidationResultList();

            foreach (var form in Steps.Select(s => s.Form))
            {
                var validationResult = form.Validate(new ValidationOptions
                {
                    ValidateFiles = options.ValidateFiles,
                    ValidateCaptcha = false
                });

                list.AddRange(validationResult);
            }

            if (options.ValidateCaptcha && RequiresCaptcha)
            {
                var requiresCaptchaAttr = new RequiresCaptchaAttribute();
                var form = new HttpContextWrapper(HttpContext.Current);

                requiresCaptchaAttr.Validate(form, list);
            }

            return list;
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
