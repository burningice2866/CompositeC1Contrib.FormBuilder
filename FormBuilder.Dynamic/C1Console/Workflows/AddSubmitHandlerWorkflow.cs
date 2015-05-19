using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Composite.C1Console.Workflow;

using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Configuration;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.Configuration;
using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class AddSubmitHandlerWorkflow : Basic1StepDialogWorkflow
    {
        private static readonly IList<SubmitHandlerElement> Handlers;

        public AddSubmitHandlerWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\AddSubmitHandlerWorkflow.xml") { }

        static AddSubmitHandlerWorkflow()
        {
            var config = FormBuilderConfiguration.GetSection();
            var plugin = (DynamicFormBuilderConfiguration)config.Plugins["dynamic"];

            Handlers = plugin.SubmitHandlers;
        }

        public static Dictionary<string, string> GetSubmitHandlerTypes()
        {
            return Handlers.ToDictionary(o => o.Type.AssemblyQualifiedName, o => o.Name);
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (!BindingExist("SubmitHandlerType"))
            {
                Bindings.Add("SubmitHandlerType", String.Empty);
                Bindings.Add("Name", String.Empty);
            }
        }

        public override bool Validate()
        {
            var token = (FormFolderEntityToken)EntityToken;
            var definition = DefinitionsFacade.GetDefinition(token.FormName);

            var name = GetBinding<string>("Name");
            if (definition.SubmitHandlers.Any(e => e.Name == name))
            {
                ShowFieldMessage("Name", "A handler with this name already exists");

                return false;
            }

            var type = GetBinding<string>("SubmitHandlerType");
            var handlerType = Type.GetType(type);
            var element = Handlers.Single(e => e.Type == handlerType);

            if (element.AllowMultiple)
            {
                return true;
            }

            if (definition.SubmitHandlers.Any(handler => handler.GetType() == handlerType))
            {
                ShowFieldMessage("SubmitHandlerType", "The chosen handler is only allowed once");

                return false;
            }

            return true;
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var token = (FormFolderEntityToken)EntityToken;

            var name = GetBinding<string>("Name");
            var type = GetBinding<string>("SubmitHandlerType");

            var definition = DefinitionsFacade.GetDefinition(token.FormName);
            var handlerType = Type.GetType(type);
            var instance = (FormSubmitHandler)Activator.CreateInstance(handlerType);

            instance.Name = name;

            definition.SubmitHandlers.Add(instance);

            DefinitionsFacade.Save(definition);

            CreateSpecificTreeRefresher().PostRefreshMesseges(token);

            var editWorkFlowAttribute = handlerType.GetCustomAttribute<EditWorkflowAttribute>();
            if (editWorkFlowAttribute != null)
            {
                var workflowToken = new WorkflowActionToken(editWorkFlowAttribute.EditWorkflowType);

                ExecuteAction(new FormSubmitHandlerEntityToken(handlerType, token.FormName, name), workflowToken);
            }
        }
    }
}
