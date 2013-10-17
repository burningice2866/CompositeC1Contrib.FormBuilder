using System.Collections.Generic;

using Composite.C1Console.Actions;
using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens
{
    [ActionExecutor(typeof(UrlActionTokenActionExecutor))]
    internal sealed class UrlActionToken : ActionToken
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
            string[] s = serializedData.Split('·');

            return new UrlActionToken(s[0], s[1], s[2].DeserializePermissionTypes());
        }
    }    
}
