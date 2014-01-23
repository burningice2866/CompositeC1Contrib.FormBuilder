using System;
using System.Linq;

using Composite.C1Console.Workflow;
using Composite.Core.Xml;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.C1Console.Tokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public class EditFormWorkflow : Basic1StepDocumentWorkflow
    {
        public EditFormWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\EditFormWorkflow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (!BindingExist("FormName"))
            {
                var formToken = (FormInstanceEntityToken)EntityToken;
                var definition = DynamicFormsFacade.GetFormByName(formToken.FormName);

                Bindings.Add("FormName", formToken.FormName);
                Bindings.Add("SubmitButtonLabel", definition.Model.SubmitButtonLabel);
                Bindings.Add("IntroText", definition.IntroText.ToString());
                Bindings.Add("SuccessResponse", definition.SuccessResponse.ToString());
                Bindings.Add("FunctionExecutor", definition.FormExecutor ?? String.Empty);
            }
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var formToken = (FormInstanceEntityToken)EntityToken;
            var definition = DynamicFormsFacade.GetFormByName(formToken.FormName);

            var formName = GetBinding<string>("FormName");
            var submitButtonLabel = GetBinding<string>("SubmitButtonLabel");
            var introText = GetBinding<string>("IntroText");
            var successResponse = GetBinding<string>("SuccessResponse");
            var functionExecutor = GetBinding<string>("FunctionExecutor");

            definition.IntroText = XhtmlDocument.Parse(introText);
            definition.SuccessResponse = XhtmlDocument.Parse(successResponse);
            definition.FormExecutor = functionExecutor;

            var submitButtonLabelAttr = definition.Model.Attributes.OfType<SubmitButtonLabelAttribute>().SingleOrDefault();
            if (submitButtonLabel != null)
            {
                definition.Model.Attributes.Remove(submitButtonLabelAttr);
            }

            if (!String.IsNullOrEmpty(submitButtonLabel))
            {
                submitButtonLabelAttr = new SubmitButtonLabelAttribute(submitButtonLabel);
                definition.Model.Attributes.Add(submitButtonLabelAttr);
            }

            if (formName != formToken.FormName)
            {
                var newDefinition = DynamicFormsFacade.CopyFormByName(formToken.FormName, formName);

                DynamicFormsFacade.SaveForm(newDefinition);

                DynamicFormsFacade.DeleteModel(definition);
            }
            else
            {
                DynamicFormsFacade.SaveForm(definition);
            }

            CreateSpecificTreeRefresher().PostRefreshMesseges(new FormElementProviderEntityToken());
            SetSaveStatus(true);
        }

        public override bool Validate()
        {
            var formToken = (FormInstanceEntityToken)EntityToken;
            var formName = GetBinding<string>("FormName");

            if (formName != formToken.FormName)
            {
                if (!FormModel.IsValidName(formName))
                {
                    ShowFieldMessage("FormName", "Form name is invalid, only a-z and 0-9 is allowed");

                    return false;
                }

                var isNameInUse = FormModelsFacade.GetModels().Any(m => m.Name == formName);
                if (isNameInUse)
                {
                    ShowFieldMessage("FormName", "Form name already exists");

                    return false;
                }
            }

            return true;
        }
    }
}
