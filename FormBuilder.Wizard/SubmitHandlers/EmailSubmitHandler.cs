using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;

using Composite.Core.WebClient;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Functions;

using CompositeC1Contrib.Email;
using CompositeC1Contrib.FormBuilder.FunctionProviders;
using CompositeC1Contrib.FormBuilder.Wizard.C1Console.Workflows;
using CompositeC1Contrib.FormBuilder.Wizard.Web;

namespace CompositeC1Contrib.FormBuilder.Wizard.SubmitHandlers
{
    [Serializable]
    [EditWorkflow(typeof(EditEmailSubmitHandlerWorkflow))]
    public class EmailSubmitHandler : FormSubmitHandler
    {
        public bool IncludeAttachments { get; set; }

        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }

        public override void Submit(FormWizardRequestContext context)
        {
            From = ResolveText(From, context);
            To = ResolveText(To, context);

            var mailMessage = new MailMessage(From, To)
            {
                Subject = ResolveText(Subject, context),
                Body = ResolveHtml(Body, context),
                IsBodyHtml = true
            };

            if (!String.IsNullOrEmpty(Cc))
            {
                mailMessage.CC.Add(ResolveText(Cc, context));
            }

            if (!String.IsNullOrEmpty(Bcc))
            {
                mailMessage.Bcc.Add(ResolveText(Bcc, context));
            }

            if (IncludeAttachments && context.StepModels.Values.Any(m => m.HasFileUpload))
            {
                var files = new List<FormFile>();

                foreach (var model in context.StepModels.Values)
                {
                    foreach (var field in model.Fields)
                    {
                        if (field.ValueType == typeof (FormFile) && field.Value != null)
                        {
                            files.Add((FormFile) field.Value);
                        }
                        else if (field.ValueType == typeof (IEnumerable<FormFile>) && field.Value != null)
                        {
                            files.AddRange((IEnumerable<FormFile>) field.Value);
                        }
                    }
                }

                foreach (var file in files)
                {
                    var attachment = new Attachment(file.InputStream, file.FileName, file.ContentType);

                    file.InputStream.Seek(0, SeekOrigin.Begin);
                    mailMessage.Attachments.Add(attachment);
                }
            }

            MailsFacade.EnqueueMessage(mailMessage);
        }

        private static string ResolveText(string text, FormWizardRequestContext context)
        {
            foreach (var model in context.StepModels.Values)
            {
                foreach (var field in model.Fields)
                {
                    if (field.Value != null)
                    {
                        text = text.Replace(String.Format("%{0}%", field.Name), field.Value.ToString());
                    }
                }
            }

            return text;
        }

        private static string ResolveHtml(string body, FormWizardRequestContext context)
        {
            XhtmlDocument templateMarkup;

            try
            {
                templateMarkup = XhtmlDocument.Parse(body);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("Failed to parse markup for file '{0}'", body), ex);
            }

            var functionContextContainer = new FunctionContextContainer(new Dictionary<string, object>
            {
                { FormWizardFunction.RenderingContextKey, context }
            });

            PageRenderer.ExecuteEmbeddedFunctions(templateMarkup.Root, functionContextContainer);

            body = templateMarkup.ToString();
            body = PageUrlHelper.ChangeRenderingPageUrlsToPublic(body);
            body = ResolveText(body, context);

            return body;
        }
    }
}