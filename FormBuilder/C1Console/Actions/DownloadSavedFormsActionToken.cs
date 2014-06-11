using System.Collections.Generic;

using Composite.C1Console.Actions;
using Composite.C1Console.Events;
using Composite.C1Console.Security;
using Composite.Core.WebClient;

namespace CompositeC1Contrib.FormBuilder.C1Console.Actions
{
    [ActionExecutor(typeof(DownloadSavedFormsActionExecutor))]
    public class DownloadSavedFormsActionToken : ActionToken
    {
        public string FormName { get; private set; }

        public override IEnumerable<PermissionType> PermissionTypes
        {
            get { return new[] { PermissionType.Edit, PermissionType.Administrate }; }
        }

        public DownloadSavedFormsActionToken(string formName)
        {
            FormName = formName;
        }

        public override string Serialize()
        {
            return FormName;
        }

        public static ActionToken Deserialize(string serializedData)
        {
            return new DownloadSavedFormsActionToken(serializedData);
        }
    }

    public class DownloadSavedFormsActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var downloadActionToken = (DownloadSavedFormsActionToken)actionToken;
            var currentConsoleId = flowControllerServicesContainer.GetService<IManagementConsoleMessageService>().CurrentConsoleId;

            var url = "InstalledPackages/CompositeC1Contrib.FormBuilder/DownloadSavedForms.ashx?form="+ downloadActionToken.FormName;
            url = UrlUtils.ResolveAdminUrl(url);

            ConsoleMessageQueueFacade.Enqueue(new DownloadFileMessageQueueItem(url), currentConsoleId);

            return null;
        }
    }
}
