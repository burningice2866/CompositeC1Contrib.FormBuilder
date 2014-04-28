using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using Composite.Core.Xml;

using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder.Wizard
{
    public class FormWizard : IFormModel
    {
        public string Name { get; set; }
        public bool ForceHttpSConnection { get; set; }
        public XhtmlDocument IntroText { get; set; }
        public XhtmlDocument SuccessResponse { get; set; }
        public IList<FormWizardStep> Steps { get; private set; }

        public IList<FormValidationRule> ValidationResult { get; private set; }
        public IDictionary<string, FormModel> StepModels { get; private set; }

        public IList<FormField> Fields
        {
            get { return StepModels.Values.SelectMany(m => m.Fields).ToList(); }
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

            StepModels = new Dictionary<string, FormModel>();
        }

        public bool ForceHttps
        {
            get { return StepModels.Values.Any(m => m.ForceHttps); }
        }

        public virtual void Submit() { }

        public void MapValues(NameValueCollection values, IEnumerable<FormFile> files)
        {
            for (int i = 0; i < Steps.Count; i++)
            {
                var nmc = new NameValueCollection();

                var step = Steps[i];
                var model = StepModels[step.Name];
                var stepPrepend = "step-" + (i + 1) + "-";
                var keys = values.AllKeys.Where(k => k.StartsWith(stepPrepend)).ToArray();

                foreach (var key in keys)
                {
                    var name = key.Remove(0, stepPrepend.Length);
                    var value = values[key];

                    nmc.Add(name, value);
                }

                model.MapValues(nmc, Enumerable.Empty<FormFile>());
                model.Validate();
            }
        }

        public void Validate()
        {
            foreach (var model in StepModels.Values)
            {
                model.Validate();

                foreach (var result in model.ValidationResult)
                {
                    ValidationResult.Add(result);
                }
            }
        }
    }
}
