using System.Collections.Generic;

using Composite.C1Console.Actions;
using Composite.C1Console.Events;
using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.C1Console.Actions
{
    [ActionExecutor(typeof(DownloadSubmittedFormsActionExecutor))]
    public class DownloadSubmittedFormsActionToken : ActionToken
    {
        public string FormName { get; private set; }

        public override IEnumerable<PermissionType> PermissionTypes
        {
            get { return new[] { PermissionType.Edit, PermissionType.Administrate }; }
        }

        public DownloadSubmittedFormsActionToken(string formName)
        {
            FormName = formName;
        }

        public override string Serialize()
        {
            return FormName;
        }

        public static ActionToken Deserialize(string serializedData)
        {
            return new DownloadSubmittedFormsActionToken(serializedData);
        }
    }

    public class DownloadSubmittedFormsActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var downloadActionToken = (DownloadSubmittedFormsActionToken)actionToken;
            var currentConsoleId = flowControllerServicesContainer.GetService<IManagementConsoleMessageService>().CurrentConsoleId;
            var url = "/formbuilder/" + downloadActionToken.FormName + "/submits.csv";

            ConsoleMessageQueueFacade.Enqueue(new DownloadFileMessageQueueItem(url), currentConsoleId);

            return null;
        }
    }
}
