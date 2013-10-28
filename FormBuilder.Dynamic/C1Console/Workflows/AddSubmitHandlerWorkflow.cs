using System;
using System.Collections.Generic;
using System.Workflow.Activities;

using Composite.C1Console.Workflow;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;
using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public sealed partial class AddSubmitHandlerWorkflow : Composite.C1Console.Workflow.Activities.FormsWorkflow
    {
        public AddSubmitHandlerWorkflow()
        {
            InitializeComponent();
        }

        public static Dictionary<string, string> GetSubmitHandlerTypes()
        {
            return new Dictionary<string, string>
            {
                { typeof(EmailSubmitHandler).AssemblyQualifiedName, "Send email" }
            };
        }

        private void validateSave(object sender, ConditionalEventArgs e)
        {
            e.Result = true;
        }

        private void initCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            if (!BindingExist("SubmitHandlerType"))
            {
                Bindings.Add("SubmitHandlerType", String.Empty);
                Bindings.Add("Name", String.Empty);
            }
        }

        private void saveCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            var token = (FormFolderEntityToken)EntityToken;

            var name = GetBinding<string>("Name");
            var type = GetBinding<string>("SubmitHandlerType");

            var definition = DynamicFormsFacade.GetFormByName(token.FormName);
            var handlerType = Type.GetType(type);
            var instance = (FormSubmitHandler)Activator.CreateInstance(handlerType);

            instance.Name = name;

            definition.SubmitHandlers.Add(instance);

            DynamicFormsFacade.SaveForm(definition);

            CreateSpecificTreeRefresher().PostRefreshMesseges(token);

            if (handlerType == typeof(EmailSubmitHandler))
            {
                var workflowToken = new WorkflowActionToken(typeof(EditEmailSubmitHandlerWorkflow));

                ExecuteAction(new FormSubmitHandlerEntityToken(handlerType, token.FormName, name), workflowToken);
            }
        }
    }
}
