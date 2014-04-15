using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder.Wizard.Web
{
    public class FormWizardRequestContext
    {
        private readonly HttpContextBase _ctx = new HttpContextWrapper(HttpContext.Current);

        public string WizardName { get; private set; }
        public FormWizard Wizard { get; private set; }

        public IDictionary<string, FormModel> StepModels { get; private set; }
        public List<FormValidationRule> ValidationResult { get; private set; }

        public bool IsOwnSubmit
        {
            get { return _ctx.Request.RequestType == "POST"; }
        }

        public bool IsSuccess
        {
            get
            {
                if (!IsOwnSubmit)
                {
                    return false;
                }

                if (ValidationResult == null)
                {
                    Execute();
                }

                return !ValidationResult.Any();
            }
        }

        public FormWizardRequestContext(string wizardName)
        {
            WizardName = wizardName;
            Wizard = FormWizardsFacade.GetWizard(WizardName);
            StepModels = new Dictionary<string, FormModel>();
        }

        private void Execute()
        {
            ValidationResult = new List<FormValidationRule>();

            var steps = Wizard.Steps;
            for (int i = 0; i < steps.Count; i++)
            {
                var step = steps[i];
                var model = FormModelsFacade.GetModel(step.FormName);
                var stepPrepend = "step-" + (i + 1) + "-";
                var keys = _ctx.Request.Form.AllKeys.Where(k => k.StartsWith(stepPrepend)).ToArray();
                var nmc = new NameValueCollection();
                foreach (var key in keys)
                {
                    var name = key.Remove(0, stepPrepend.Length);
                    var value = _ctx.Request.Form[key];

                    nmc.Add(name, value);
                }

                model.MapValues(nmc, Enumerable.Empty<FormFile>());
                model.Validate();

                ValidationResult.AddRange(model.ValidationResult);

                StepModels.Add(step.Name, model);
            }
        }

        public void Submit()
        {
            foreach (var handler in Wizard.SubmitHandlers)
            {
                handler.Submit(this);
            }
        }
    }
}
