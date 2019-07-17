using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Xml.Linq;

using Composite.Data;

using CompositeC1Contrib.Email;
using CompositeC1Contrib.Email.Data.Types;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers
{
    [Serializable]
    [EditWorkflow(typeof(EditEmailSubmitHandlerWorkflow))]
    public class EmailSubmitHandler : FormSubmitHandler
    {
        public bool IncludeAttachments { get; set; }

        protected IEnumerable<FormFile> GetFiles(IModelInstance instance)
        {
            var files = new List<FormFile>();

            foreach (var field in instance.Fields)
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

        public override void Save(IDynamicDefinition definition, XElement handler)
        {
            handler.Add(new XAttribute("IncludeAttachments", IncludeAttachments));

            base.Save(definition, handler);
        }

        public override void Delete(IDynamicDefinition definition)
        {
            using (var data = new DataConnection())
            {
                var templates = data.Get<IMailTemplate>().Where(t => t.Key == definition.Name + "." + Name);
                var templateContents = data.Get<IMailTemplateContent>().Where(t => t.TemplateKey == definition.Name + "." + Name);

                data.Delete(templates);
                data.Delete(templateContents);
            }

            base.Delete(definition);
        }

        public override void Submit(IModelInstance instance)
        {
            using (var data = new DataConnection())
            {
                var templateKey = instance.Name + "." + Name;
                var template = data.Get<IMailTemplate>().Single(t => t.Key == templateKey);
                var builder = new FormModelMailMessageBuilder(template, instance);

                if (IncludeAttachments && instance.HasFileUpload)
                {
                    var files = GetFiles(instance);

                    AddFilesToMessage(files, builder);
                }

                using (var msg = builder.BuildMailMessage())
                {
                    MailsFacade.EnqueueMessage(msg);
                }
            }
        }
    }
}
