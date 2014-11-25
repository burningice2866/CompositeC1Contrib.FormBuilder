using System;
using System.Linq;

using Composite.C1Console.Workflow;

using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class AddFormWizardWorkflow : BaseAddFormWorkflow
    {
        public AddFormWizardWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\AddFormWizardWorkflow.xml") { }

        public override void OnFinish(object sender, EventArgs e)
        {
            var name = GetBinding<string>("Name");

            var wizard = new DynamicFormWizard
            {
                Name = name
            };

            DynamicFormWizardsFacade.SaveWizard(wizard);

            var token = new FormInstanceEntityToken(typeof(FormBuilderElementProvider).Name, wizard.Name);
            var workflowToken = new WorkflowActionToken(typeof(EditFormWizardWorkflow));

            CreateAddNewTreeRefresher(EntityToken).PostRefreshMesseges(token);
            ExecuteAction(token, workflowToken);
        }

        public override bool Validate()
        {
            var name = GetBinding<string>("Name");

            var valid = base.Validate();
            if (!valid)
            {
                return false;
            }

            var isNameInUse = DefinitionsFacade.GetDefinitions().Any(m => m.Name == name);
            if (isNameInUse)
            {
                ShowFieldMessage("Name", "Wizard name already exists");

                return false;
            }

                return true;
            }
        }
    }
