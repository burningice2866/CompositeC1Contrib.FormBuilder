using System.Collections.Generic;
using System.Linq;

namespace CompositeC1Contrib.FormBuilder
{
    public sealed class WizardModel : IModel
    {
        public string Name { get; }
        public bool RequiresCaptcha { get; set; }
        public bool ForceHttps { get; set; }
        public IList<WizardStepModel> Steps { get; }

        public IList<FormFieldModel> Fields
        {
            get { return Steps.Select(s => s.FormModel).SelectMany(m => m.Fields).ToList(); }
        }

        public bool DisableAntiForgery
        {
            get { return Steps.Select(s => s.FormModel).Any(m => m.DisableAntiForgery); }
        }

        public bool HasFileUpload
        {
            get { return Fields.Any(f => f.ValueType == typeof(FormFile) || f.ValueType == typeof(IEnumerable<FormFile>)); }
        }

        public WizardModel(string name)
        {
            Name = name;
            Steps = new List<WizardStepModel>();
        }
    }
}
