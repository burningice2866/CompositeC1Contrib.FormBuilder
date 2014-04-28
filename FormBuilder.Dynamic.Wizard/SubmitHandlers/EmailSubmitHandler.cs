using System;
using System.Collections.Generic;
using System.Linq;

using Composite.Data;

using CompositeC1Contrib.Email;
using CompositeC1Contrib.Email.Data.Types;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;
using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;
using CompositeC1Contrib.FormBuilder.Wizard;

using EditEmailSubmitHandlerWorkflow = CompositeC1Contrib.FormBuilder.Dynamic.Wizard.C1Console.Workflows.EditEmailSubmitHandlerWorkflow;

namespace CompositeC1Contrib.FormBuilder.Dynamic.Wizard.SubmitHandlers
{
    [Serializable]
    [EditWorkflow(typeof(EditEmailSubmitHandlerWorkflow))]
    public class EmailSubmitHandler : BaseDynamicEmailSubmitHandler
    {
        public override void Submit(IFormModel definition)
        {
            var model = (DynamicFormWizard)definition;

            using (var data = new DataConnection())
            {
                var templateKey = model.Name + "." + Name;
                var template = data.Get<IMailTemplate>().Single(t => t.Key == templateKey);
                var builder = new FormWizardMailMessageBuilder(template, model);

                if (IncludeAttachments && model.StepModels.Values.Any(m => m.HasFileUpload))
                {
                    var files = new List<FormFile>();

                    foreach (var formModel in model.StepModels.Values)
                    {
                        files.AddRange(GetFiles(formModel));
                    }

                    AddFilesToMessage(files, builder);
                }

                var msg = builder.BuildMailMessage();

                MailsFacade.EnqueueMessage(msg);
            }
        }
    }
}