using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Actions;
using Composite.C1Console.Security;
using Composite.Core.Xml;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Actions
{
    [ActionExecutor(typeof(DeleteFormFieldActionExecutor))]
    public class DeleteFormFieldActionToken : ActionToken
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

    public class DeleteFormFieldActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var fieldToken = (FormFieldEntityToken)entityToken;
            var definition = DynamicFormsFacade.GetFormByName(fieldToken.FormName);
            var field = definition.Model.Fields.Single(f => f.Name == fieldToken.FieldName);

            definition.Model.Fields.Remove(field);

            if (RenderingLayoutFacade.HasCustomRenderingLayout(fieldToken.FormName))
            {
                var layout = RenderingLayoutFacade.GetRenderingLayout(fieldToken.FormName);
                var fieldElement = layout.Body.Descendants().SingleOrDefault(el => el.Name == Namespaces.Xhtml + "p" && el.Value.Trim() == "%" + field.Name + "%");

                if (fieldElement != null)
                {
                    fieldElement.Remove();
                }

                RenderingLayoutFacade.SaveRenderingLayout(fieldToken.FormName, layout);
            }

            DynamicFormsFacade.SaveForm(definition);

            new ParentTreeRefresher(flowControllerServicesContainer).PostRefreshMesseges(entityToken);

            return null;
        }
    }
}
