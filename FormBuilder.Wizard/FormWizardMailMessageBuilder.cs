using System.Collections.Generic;
using System.Linq;

using Composite.Functions;

using CompositeC1Contrib.Email;
using CompositeC1Contrib.Email.Data.Types;
using CompositeC1Contrib.FormBuilder.FunctionProviders;

namespace CompositeC1Contrib.FormBuilder.Wizard
{
    public class FormWizardMailMessageBuilder : MailMessageBuilder
    {
        private readonly FormWizard _wizard;

        public FormWizardMailMessageBuilder(IMailTemplate template, FormWizard wizard) : base(template)
        {
            _wizard = wizard;
        }

        protected override IDictionary<string, object> GetDictionaryFromModel()
        {
            return _wizard.StepModels.Values.SelectMany(s => s.Fields).Where(f => f.Value != null).ToDictionary(f => f.Name, f => f.Value);
        }

        protected override string ResolveHtml(string body)
        {
            var functionContextContainer = new FunctionContextContainer(new Dictionary<string, object>
            {
                { BaseFormFunction.FormModelKey, _wizard }
            });

            return ResolveHtml(body, functionContextContainer, ResolveText);
        }
    }
}
