using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Workflow;

using CompositeC1Contrib.FormBuilder.Configuration;
using CompositeC1Contrib.FormBuilder.Wizard.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Wizard.Configuration;
using CompositeC1Contrib.FormBuilder.Wizard.SubmitHandlers;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Wizard.C1Console.Workflows
{
    public class AddSubmitHandlerWorkflow : Basic1StepDialogWorkflow
    {
        private static readonly IList<SubmitHandlerElement> Handlers;

        public AddSubmitHandlerWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Wizard\\AddSubmitHandlerWorkflow.xml") { }

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
            if (!BindingExist("SubmitHandlerType"))
            {
                Bindings.Add("SubmitHandlerType", String.Empty);
                Bindings.Add("Name", String.Empty);
            }
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
            var wizard = FormWizardsFacade.GetWizard(token.WizardName);

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

            var wizard = FormWizardsFacade.GetWizard(token.WizardName);
            var handlerType = Type.GetType(type);
            var instance = (FormSubmitHandler)Activator.CreateInstance(handlerType);

            instance.Name = name;

            wizard.SubmitHandlers.Add(instance);

            FormWizardsFacade.SaveWizard(wizard);

            CreateSpecificTreeRefresher().PostRefreshMesseges(token);

            if (typeof(EmailSubmitHandler).IsAssignableFrom(handlerType))
            {
                var workflowToken = new WorkflowActionToken(typeof(EditEmailSubmitHandlerWorkflow));

                ExecuteAction(new FormSubmitHandlerEntityToken(handlerType, token.WizardName, name), workflowToken);
            }
        }
    }
}
