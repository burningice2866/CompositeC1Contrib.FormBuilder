using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

using Composite.Core.WebClient;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Functions;

using CompositeC1Contrib.Email;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers
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

        public override void Submit(FormModel model)
        {
                From = ResolveText(From, model);
                To = ResolveText(To, model);

                var mailMessage = new MailMessage(From, To)
                {
                    Subject = ResolveText(Subject, model),
                    Body = ResolveHtml(Body, model),
                    IsBodyHtml = true
                };

                if (!String.IsNullOrEmpty(Cc))
                {
                    mailMessage.CC.Add(ResolveText(Cc, model));
                }

                if (!String.IsNullOrEmpty(Bcc))
                {
                    mailMessage.Bcc.Add(ResolveText(Bcc, model));
                }

                if (IncludeAttachments && model.HasFileUpload)
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

                    foreach (var file in files)
                    {
                        var attachment = new Attachment(file.InputStream, file.FileName, file.ContentType);

                        file.InputStream.Seek(0, SeekOrigin.Begin);
                        mailMessage.Attachments.Add(attachment);
                    }
                }

            MailsFacade.EnqueueMessage(mailMessage);
        }

        private static string ResolveText(string text, FormModel model)
        {
            foreach (var field in model.Fields)
            {
                if (field.Value != null)
                {
                    text = text.Replace(String.Format("%{0}%", field.Name), field.Value.ToString());
                }
            }

            return text;
        }

        private static string ResolveHtml(string body, FormModel model)
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
                { "FormModel", model }
            });

            PageRenderer.ExecuteEmbeddedFunctions(templateMarkup.Root, functionContextContainer);

            body = templateMarkup.ToString();
            body = PageUrlHelper.ChangeRenderingPageUrlsToPublic(body);
            body = ResolveText(body, model);

            return body;
        }
    }
}