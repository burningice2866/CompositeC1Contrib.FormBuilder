using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Xml.Linq;

using Composite.Core.Xml;
using Composite.Data;

using CompositeC1Contrib.Email;
using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers
{
    public abstract class BaseDynamicEmailSubmitHandler : FormSubmitHandler
    {
        public SerializableMailTemplate MailTemplate { get; private set; }
        public bool IncludeAttachments { get; set; }

        protected IEnumerable<FormFile> GetFiles(IFormModel model)
        {
            var files = new List<FormFile>();

            foreach (var field in model.Fields)
            {
                if (field.ValueType == typeof(FormFile) && field.Value != null)
                {
                    files.Add((FormFile)field.Value);
                }
                else if (field.ValueType == typeof(IEnumerable<FormFile>) && field.Value != null)
                {
                    files.AddRange((IEnumerable<FormFile>)field.Value);
                }
            }

            return files;
        }

        protected void AddFilesToMessage(IEnumerable<FormFile> files, MailMessageBuilder builder)
        {
            foreach (var file in files)
            {
                var attachment = new Attachment(file.InputStream, file.FileName, file.ContentType);

                file.InputStream.Seek(0, SeekOrigin.Begin);
                builder.AddAttachment(attachment);
            }
        }

        public override void Load(IDynamicFormDefinition definition, XElement handler)
        {
            using (var data = new DataConnection())
            {
                var templateKey = definition.Name + "." + Name;
                var template = data.Get<IMailTemplate>().SingleOrDefault(t => t.Key == templateKey);

                if (template == null)
                {
                    template = data.CreateNew<IMailTemplate>();

                    template.Key = templateKey;
                    template.From = handler.Attribute("From").GetValueOrDefault(String.Empty);
                    template.To = handler.Attribute("To").GetValueOrDefault(String.Empty);
                    template.Cc = handler.Attribute("Cc").GetValueOrDefault(String.Empty);
                    template.Bcc = handler.Attribute("Bcc").GetValueOrDefault(String.Empty);
                    template.Subject = handler.Element("Subject") != null ? handler.Element("Subject").Value : String.Empty;
                    template.Body = handler.Element("Body") != null ? handler.Element("Body").Value : String.Empty;
                    template.EncryptMessage = false;
                    template.EncryptPassword = String.Empty;

                    data.Add(template);
                }

                MailTemplate = SerializableMailTemplate.FromData(template);
            }

            base.Load(definition, handler);
        }

        public override void Save(IDynamicFormDefinition definition, XElement handler)
        {
            handler.Add(new XAttribute("IncludeAttachments", IncludeAttachments));

            if (MailTemplate != null)
            {
                using (var data = new DataConnection())
                {
                    var add = false;
                    var templateKey = definition.Name + "." + Name;

                    var template = data.Get<IMailTemplate>().SingleOrDefault(t => t.Key == templateKey);
                    if (template == null)
                    {
                        add = true;

                        template = data.CreateNew<IMailTemplate>();
                    }

                    template.Key = templateKey;
                    template.From = MailTemplate.From;
                    template.To = MailTemplate.To;
                    template.Cc = MailTemplate.Cc;
                    template.Bcc = MailTemplate.Bcc;
                    template.Subject = MailTemplate.Subject;
                    template.Body = MailTemplate.Body;
                    template.EncryptMessage = MailTemplate.EncryptMessage;
                    template.EncryptPassword = MailTemplate.EncryptPassword;

                    if (add)
                    {
                        data.Add(template);

                    }
                    else
                    {
                        data.Update(template);
                    }
                }
            }

            base.Save(definition, handler);
        }

        public override void Delete(IDynamicFormDefinition definition)
        {
            using (var data = new DataConnection())
            {
                var templates = data.Get<IMailTemplate>().Where(t => t.Key == definition.Name +"."+ Name);

                data.Delete<IMailTemplate>(templates);
            }

            base.Delete(definition);
        }
    }
}
