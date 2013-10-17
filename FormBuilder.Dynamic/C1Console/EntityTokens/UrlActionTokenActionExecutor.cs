using System;

using Composite.C1Console.Actions;
using Composite.C1Console.Events;
using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens
{
    public class UrlActionTokenActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var urlActionToken = (UrlActionToken)actionToken;

            string currentConsoleId = flowControllerServicesContainer.GetService<IManagementConsoleMessageService>().CurrentConsoleId;
            string seperator = urlActionToken.Url.Contains("?") ? "&" : "?";

            var openViewMessageItem = new OpenViewMessageQueueItem
            {
                Label = urlActionToken.Label,
                Url = urlActionToken.Url + seperator + "consoleId=" + currentConsoleId +"&EntityToken="+ EntityTokenSerializer.Serialize(entityToken),
                ViewId = Guid.NewGuid().ToString(),
                ViewType = ViewType.Main
            };

            ConsoleMessageQueueFacade.Enqueue(openViewMessageItem, currentConsoleId);

            return null;
        }
    }
}
