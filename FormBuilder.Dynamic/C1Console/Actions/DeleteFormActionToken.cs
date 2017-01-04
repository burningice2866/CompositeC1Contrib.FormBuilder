using System;
using System.Collections.Generic;

using Composite.C1Console.Actions;
using Composite.C1Console.Security;
using Composite.Data;

using CompositeC1Contrib.FormBuilder.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Data.Types;
using CompositeC1Contrib.Localization;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions
{
    [ActionExecutor(typeof(DeleteFormActionExecutor))]
    public class DeleteFormActionToken : ActionToken
    {
        private static readonly IEnumerable<PermissionType> _permissionTypes = new[] { PermissionType.Delete };

        public override IEnumerable<PermissionType> PermissionTypes
        {
            get { return _permissionTypes; }
        }

        public override string Serialize()
        {
            return String.Empty;
        }

        public static ActionToken Deserialize(string serializedData)
        {
            return new DeleteFormFieldActionToken();
        }
    }

    public class DeleteFormActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var modelReference = (IModelReference)((DataEntityToken)entityToken).Data;
            var definition = DefinitionsFacade.GetDefinition(modelReference.Name);

            DefinitionsFacade.Delete(definition);
            LocalizationsFacade.DeleteNamespace(Localization.GenerateKey(modelReference.Name));

            var ns = modelReference.Name.Substring(0, modelReference.Name.LastIndexOf("."));

            new SpecificTreeRefresher(flowControllerServicesContainer).PostRefreshMesseges(new NamespaceFolderEntityToken(typeof(FormBuilderElementProvider).Name, ns));

            return null;
        }
    }
}
