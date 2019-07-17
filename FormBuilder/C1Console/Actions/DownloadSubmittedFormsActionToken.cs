using System;
using System.Collections.Generic;

using Composite.C1Console.Actions;
using Composite.C1Console.Events;
using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.C1Console.Actions
{
    [ActionExecutor(typeof(DownloadSubmittedFormsActionExecutor))]
    public class DownloadSubmittedFormsActionToken : ActionToken
    {
        public string FormName { get; }
        public string Extension { get; }

        public override IEnumerable<PermissionType> PermissionTypes => new[] { PermissionType.Read };

        public DownloadSubmittedFormsActionToken(string formName, string extension)
        {
            FormName = formName;
            Extension = extension;
        }

        public override string Serialize()
        {
            return FormName + "?" + Extension;
        }

        public static ActionToken Deserialize(string serializedData)
        {
            var split = serializedData.Split(new[] { '?' }, StringSplitOptions.RemoveEmptyEntries);

            return new DownloadSubmittedFormsActionToken(split[0], split[1]);
        }
    }

    public class DownloadSubmittedFormsActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var downloadActionToken = (DownloadSubmittedFormsActionToken)actionToken;
            var currentConsoleId = flowControllerServicesContainer.GetService<IManagementConsoleMessageService>().CurrentConsoleId;
            var url = "/formbuilder/" + downloadActionToken.FormName + "/submits" + downloadActionToken.Extension;

            ConsoleMessageQueueFacade.Enqueue(new DownloadFileMessageQueueItem(url), currentConsoleId);

            return null;
        }
    }
}
