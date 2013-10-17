using System;
using System.Text;

using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.Serialization;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class ConfirmWorkflowActionToken : WorkflowActionToken
    {
        public ConfirmWorkflowActionToken(string confirmMessage, Type type) :
            base(typeof(ConfirmWorkflow), new PermissionType[] { PermissionType.Administrate })
        {
            var sb = new StringBuilder();

            StringConversionServices.SerializeKeyValuePair(sb, "ConfirmMessage", confirmMessage);
            StringConversionServices.SerializeKeyValuePair<Type>(sb, "Type", type);
            
            Payload = sb.ToString();
        }

        public new static ActionToken Deserialize(string serialiedWorkflowActionToken)
        {
            return WorkflowActionToken.Deserialize(serialiedWorkflowActionToken);
        }
    }
}
