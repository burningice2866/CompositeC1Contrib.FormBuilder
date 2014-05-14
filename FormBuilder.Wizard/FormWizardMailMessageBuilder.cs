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
            var dict = new Dictionary<string, object>();

            foreach (var step in _wizard.StepModels)
            {
                var fields = step.Value.Fields.Where(f => f.Value != null);

                foreach (var field in fields)
                {
                    dict.Add(step.Key +"_"+ field.Name, field.Value);
                }
            }

            return dict;
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
