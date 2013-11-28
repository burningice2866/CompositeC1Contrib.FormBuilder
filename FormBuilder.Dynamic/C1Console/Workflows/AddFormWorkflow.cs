using System;
using System.Linq;
using System.Workflow.Activities;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class AddFormWorkflow : Basic1StepAddDialogWorkflow
    {
        public override string FormDefinitionFileName
        {
            get { return "\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\AddFormWorkflow.xml"; }
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (!BindingExist("FormName"))
            {
                Bindings.Add("FormName", String.Empty);
            }
        }

        public override void OnSave(object sender, EventArgs e)
        {
            var formName = GetBinding<string>("FormName");
            var model = new DynamicFormDefinition(formName);

            DynamicFormsFacade.SaveForm(model);

            var treeRefresher = CreateAddNewTreeRefresher(EntityToken);
            treeRefresher.PostRefreshMesseges(new FormInstanceEntityToken(typeof(FormBuilderElementProvider).Name, formName));
        }

        public override void OnValidate(object sender, ConditionalEventArgs e)
        {
            var formName = GetBinding<string>("FormName");

            if (!FormModel.IsValidName(formName))
            {
                ShowFieldMessage("FormName", "Form name is invalid, only a-z and 0-9 is allowed");

                e.Result = false;
                return;
            }

            var isNameInUse = FormModelsFacade.GetModels().Any(m => m.Name == formName);
            if (isNameInUse)
            {
                ShowFieldMessage("FormName", "Form name already exists");

                e.Result = false;
                return;
            }

            base.OnValidate(sender, e);
        }
    }
}
