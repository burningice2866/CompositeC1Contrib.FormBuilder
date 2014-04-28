using System;
using System.Linq;

using Composite.C1Console.Security;
using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;
using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public abstract class BaseEditEmailSubmitHandlerWorkflow<T> : Basic1StepDocumentWorkflow where T : EntityToken
    {
        protected T SubmitHandlerEntityToken
        {
            get { return (T) EntityToken; }
        }

        protected BaseEditEmailSubmitHandlerWorkflow(string packageName) : base("\\InstalledPackages\\" + packageName + "\\EditEmailSubmitHandlerWorkflow.xml") { }

        protected abstract BaseDynamicEmailSubmitHandler GetHandler();
        protected abstract IDynamicFormDefinition GetDefinition();
        protected abstract void SaveDefintion(IDynamicFormDefinition defintDefinition);

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("Name"))
            {
                return;
            }

            var definition = GetDefinition();
            var handler = GetHandler();

            using (var data = new DataConnection())
            {
                var template = data.Get<IMailTemplate>().Single(t => t.Key == definition.Name + "." + handler.Name);

                Bindings.Add("Name", handler.Name);
                Bindings.Add("IncludeAttachments", handler.IncludeAttachments);

                Bindings.Add("From", template.From);
                Bindings.Add("To", template.To);
                Bindings.Add("Cc", template.Cc);
                Bindings.Add("Bcc", template.Bcc);
                Bindings.Add("Subject", template.Subject);
                Bindings.Add("Body", template.Body);
                Bindings.Add("EncryptMessage", template.EncryptMessage);
                Bindings.Add("EncryptPassword", template.EncryptPassword);
            }
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var name = GetBinding<string>("Name");
            var includeAttachments = GetBinding<bool>("IncludeAttachments");

            var from = GetBinding<string>("From");
            var to = GetBinding<string>("To");
            var cc = GetBinding<string>("Cc");
            var bcc = GetBinding<string>("Bcc");
            var subject = GetBinding<string>("Subject");
            var body = GetBinding<string>("Body");
            var encryptMessage = GetBinding<bool>("EncryptMessage");
            var encryptPassword = GetBinding<string>("EncryptPassword");

            var definition = GetDefinition();
            var handler = GetHandler();
            var existingHandler = definition.SubmitHandlers.Single(h => h.Name == handler.Name);

            handler.Name = name;
            handler.IncludeAttachments = includeAttachments;
            handler.MailTemplate.From = from;
            handler.MailTemplate.To = to;
            handler.MailTemplate.Cc = cc;
            handler.MailTemplate.Bcc = bcc;
            handler.MailTemplate.Subject = subject;
            handler.MailTemplate.Body = body;
            handler.MailTemplate.EncryptMessage = encryptMessage;
            handler.MailTemplate.EncryptPassword = encryptPassword;

            definition.SubmitHandlers.Remove(existingHandler);
            definition.SubmitHandlers.Add(handler);

            SaveDefintion(definition);

            CreateSpecificTreeRefresher().PostRefreshMesseges(EntityToken);
            SetSaveStatus(true);
        }

        public override bool Validate()
        {
            var handler = GetHandler();

            var name = GetBinding<string>("Name");
            if (name != handler.Name)
            {
                var definition = GetDefinition();
                if (definition.SubmitHandlers.Any(h => h.Name == name))
                {
                    ShowFieldMessage("Name", "Handler name already exists");

                    return false;
                }
            }

            return true;
        }
    }
}
