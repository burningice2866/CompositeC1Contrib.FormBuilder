using System.Collections.Generic;
using System.Linq;

using Composite.Functions;

using CompositeC1Contrib.Email;
using CompositeC1Contrib.Email.Data.Types;
using CompositeC1Contrib.FormBuilder.FunctionProviders;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public class FormModelMailMessageBuilder : MailMessageBuilder
    {
        private readonly IFormModel _model;

        public FormModelMailMessageBuilder(IMailTemplate template, IFormModel model) : base(template)
        {
            _model = model;
        }

        protected override IDictionary<string, object> GetDictionaryFromModel()
        {
            return _model.Fields.Where(f => f.Value != null).ToDictionary(f => f.Name, f => f.Value);
        }

        protected override string ResolveHtml(string body)
        {
            var functionContextContainer = new FunctionContextContainer(new Dictionary<string, object>
            {
                { BaseFormFunction.FormModelKey, _model }
            });

            return ResolveHtml(body, functionContextContainer, ResolveText);
        }
    }
}
