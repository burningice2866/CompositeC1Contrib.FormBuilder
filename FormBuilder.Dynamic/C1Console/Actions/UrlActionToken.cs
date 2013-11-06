using System;
using System.Collections.Generic;

using Composite.C1Console.Actions;
using Composite.C1Console.Events;
using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions
{
    [ActionExecutor(typeof(UrlActionTokenActionExecutor))]
    public sealed class UrlActionToken : ActionToken
    {
        private IEnumerable<PermissionType> _permissionTypes;

        public string Label { get; private set; }
        public string Url { get; private set; }

        public UrlActionToken(string label, string url, IEnumerable<PermissionType> permissionTypes)
        {
            Label = label;
            Url = url;
            _permissionTypes = permissionTypes;
        }

        public override IEnumerable<PermissionType> PermissionTypes
        {
            get { return _permissionTypes; }
        }
        
        public override string Serialize()
        {
            return Label + "·" + Url + "·" + PermissionTypes.SerializePermissionTypes();
        }

        public static ActionToken Deserialize(string serializedData)
        {
            var s = serializedData.Split('·');

            return new UrlActionToken(s[0], s[1], s[2].DeserializePermissionTypes());
        }
    }

    public class UrlActionTokenActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var urlActionToken = (UrlActionToken)actionToken;

            var currentConsoleId = flowControllerServicesContainer.GetService<IManagementConsoleMessageService>().CurrentConsoleId;
            var seperator = urlActionToken.Url.Contains("?") ? "&" : "?";

            var openViewMessageItem = new OpenViewMessageQueueItem
            {
                Label = urlActionToken.Label,
                Url = urlActionToken.Url + seperator + "consoleId=" + currentConsoleId + "&EntityToken=" + EntityTokenSerializer.Serialize(entityToken),
                ViewId = Guid.NewGuid().ToString(),
                ViewType = ViewType.Main
            };

            ConsoleMessageQueueFacade.Enqueue(openViewMessageItem, currentConsoleId);

            return null;
        }
    }
}
