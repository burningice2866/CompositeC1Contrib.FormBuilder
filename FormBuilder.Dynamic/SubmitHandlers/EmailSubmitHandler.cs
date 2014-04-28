using System;
using System.Collections.Generic;
using System.Linq;

using Composite.Data;

using CompositeC1Contrib.Email;
using CompositeC1Contrib.Email.Data.Types;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers
{
    [Serializable]
    [EditWorkflow(typeof(EditEmailSubmitHandlerWorkflow))]
    public class EmailSubmitHandler : BaseDynamicEmailSubmitHandler
    {
        public override void Submit(IFormModel model)
        {
            using (var data = new DataConnection())
            {
                var templateKey = model.Name + "." + Name;
                var template = data.Get<IMailTemplate>().Single(t => t.Key == templateKey);
                var builder = new FormModelMailMessageBuilder(template, model);

                if (IncludeAttachments && model.HasFileUpload)
                {
                    var files = new List<FormFile>();

                    GetFiles(model);
                    AddFilesToMessage(files, builder);
                }

                var msg = builder.BuildMailMessage();

                MailsFacade.EnqueueMessage(msg);
            }
        }
    }
}