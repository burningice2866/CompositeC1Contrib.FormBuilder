using System;

using Composite.C1Console.Users;
using Composite.C1Console.Workflow;
using Composite.Core.Xml;
using Composite.Data;

using CompositeC1Contrib.FormBuilder.Data.Types;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public class EditFormRenderingLayoutWorkflow : Basic1StepDocumentWorkflow
    {
        public EditFormRenderingLayoutWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder\\EditFormRenderingLayoutWorkflow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("Title"))
            {
                return;
            }

            var formToken = (IModelReference)((DataEntityToken)EntityToken).Data;
            var renderingMarkup = RenderingLayoutFacade.GetRenderingLayout(formToken.Name, UserSettings.ActiveLocaleCultureInfo);

            Bindings.Add("Title", formToken.Name + " rendering layout");
            Bindings.Add("RenderingMarkup", renderingMarkup.ToString());
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var formToken = (IModelReference)((DataEntityToken)EntityToken).Data;
            var renderingMarkup = GetBinding<string>("RenderingMarkup");

            RenderingLayoutFacade.SaveRenderingLayout(formToken.Name, XhtmlDocument.Parse(renderingMarkup), UserSettings.ActiveLocaleCultureInfo);

            SetSaveStatus(true);
        }
    }
}
