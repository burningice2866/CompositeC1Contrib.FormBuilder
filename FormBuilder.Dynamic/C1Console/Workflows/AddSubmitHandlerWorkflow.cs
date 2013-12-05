using System;
using System.Linq;
using System.Collections.Generic;
using Composite.C1Console.Workflow;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;
using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class AddSubmitHandlerWorkflow : Basic1StepDialogWorkflow
    {
        public AddSubmitHandlerWorkflow()
            : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\AddSubmitHandlerWorkflow.xml")
        {
        }

        public static Dictionary<string, string> GetSubmitHandlerTypes()
        {
            var handlers = new Dictionary<string, string>
            {
                { typeof(EmailSubmitHandler).AssemblyQualifiedName, "Send email" }
            };

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in assemblies)
            {
                try
                {
                    var types = asm.GetTypes()
                        .Where(t => t.IsClass && !t.IsAbstract && t != typeof(EmailSubmitHandler) && typeof(EmailSubmitHandler).IsAssignableFrom(t));

                    foreach (var type in types)
	                {
                        handlers.Add(type.AssemblyQualifiedName, type.Name);
	                }
                }
                catch { }
            }

            return handlers;
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (!BindingExist("SubmitHandlerType"))
            {
                Bindings.Add("SubmitHandlerType", String.Empty);
                Bindings.Add("Name", String.Empty);
            }
        }

        public override void OnFinish(object sender, EventArgs e)
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

            if (typeof(EmailSubmitHandler).IsAssignableFrom(handlerType))
            {
                var workflowToken = new WorkflowActionToken(typeof(EditEmailSubmitHandlerWorkflow));

                ExecuteAction(new FormSubmitHandlerEntityToken(handlerType, token.FormName, name), workflowToken);
            }
        }
    }
}
