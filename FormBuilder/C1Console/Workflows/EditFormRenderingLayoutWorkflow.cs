using System;

using Composite.C1Console.Workflow;
using Composite.Core.Xml;

using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public class EditFormRenderingLayoutWorkflow : Basic1StepDocumentWorkflow
    {
        public EditFormRenderingLayoutWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder\\EditFormRenderingLayoutWorkflow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (!BindingExist("Title"))
            {
                var formToken = (FormInstanceEntityToken)EntityToken;
                var renderingMarkup = RenderingLayoutFacade.GetRenderingLayout(formToken.Name);

                Bindings.Add("Title", formToken.Name + " rendering layout");
                Bindings.Add("RenderingMarkup", renderingMarkup.ToString());
            }
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var formToken = (FormInstanceEntityToken)EntityToken;
            var renderingMarkup = GetBinding<string>("RenderingMarkup");

            RenderingLayoutFacade.SaveRenderingLayout(formToken.Name, XhtmlDocument.Parse(renderingMarkup));

            SetSaveStatus(true);
        }
    }
}
