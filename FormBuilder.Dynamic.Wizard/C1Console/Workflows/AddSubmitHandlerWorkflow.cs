using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Composite.C1Console.Workflow;

using CompositeC1Contrib.FormBuilder.Configuration;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;
using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;
using CompositeC1Contrib.FormBuilder.Dynamic.Wizard.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.Wizard.Configuration;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.Wizard.C1Console.Workflows
{
    public class AddSubmitHandlerWorkflow : Basic1StepDialogWorkflow
    {
        private static readonly IList<SubmitHandlerElement> Handlers;

        public AddSubmitHandlerWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic.Wizard\\AddSubmitHandlerWorkflow.xml") { }

        static AddSubmitHandlerWorkflow()
        {
            var config = FormBuilderConfiguration.GetSection();
            var plugin = (WizardFormBuilderConfiguration)config.Plugins["wizard"];

            Handlers = plugin.SubmitHandlers;
        }

        public static Dictionary<string, string> GetSubmitHandlerTypes()
        {
            return Handlers.ToDictionary(o => o.Type.AssemblyQualifiedName, o => o.Name);
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("SubmitHandlerType"))
            {
                return;
            }

            Bindings.Add("SubmitHandlerType", String.Empty);
            Bindings.Add("Name", String.Empty);
        }

        public override bool Validate()
        {
            var type = GetBinding<string>("SubmitHandlerType");
            var handlerType = Type.GetType(type);
            var element = Handlers.Single(e => e.Type == handlerType);

            if (element.AllowMultiple)
            {
                return true;
            }

            var token = (FormWizardFolderEntityToken)EntityToken;
            var wizard = DynamicFormWizardsFacade.GetWizard(token.WizardName);

            if (wizard.SubmitHandlers.Any(handler => handler.GetType() == handlerType))
            {
                ShowFieldMessage("SubmitHandlerType", "The chosen handler is only allowed once");

                return false;
            }

            return true;
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var token = (FormWizardFolderEntityToken)EntityToken;

            var name = GetBinding<string>("Name");
            var type = GetBinding<string>("SubmitHandlerType");

            var wizard = DynamicFormWizardsFacade.GetWizard(token.WizardName);
            var handlerType = Type.GetType(type);
            var instance = (FormSubmitHandler)Activator.CreateInstance(handlerType);

            instance.Name = name;

            wizard.SubmitHandlers.Add(instance);

            DynamicFormWizardsFacade.SaveWizard(wizard);

            CreateSpecificTreeRefresher().PostRefreshMesseges(token);

            var editWorkFlowAttribute = handlerType.GetCustomAttribute<EditWorkflowAttribute>();
            if (editWorkFlowAttribute == null)
            {
                return;
            }

            var workflowToken = new WorkflowActionToken(editWorkFlowAttribute.EditWorkflowType);

            ExecuteAction(new FormSubmitHandlerEntityToken(handlerType, token.WizardName, name), workflowToken);
        }
    }
}
