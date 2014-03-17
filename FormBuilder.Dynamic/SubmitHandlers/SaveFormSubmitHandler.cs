using System;
using Composite.C1Console.Elements;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers
{
    [Serializable]
    public class SaveFormSubmitHandler : FormSubmitHandler, IActionPrivider
    {
        public bool IncludeAttachments { get; set; }

        public override void Submit(FormModel model)
        {
            SaveFormSubmitFacade.SaveSubmit(model, IncludeAttachments);
        }

        public void AddActions(DynamicFormDefinition definition, Element element)
        {
            var downloadActionToken = new DownloadSavedFormsActionToken(definition.Name);
            element.AddAction(new ElementAction(new ActionHandle(downloadActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Download saved forms",
                    ToolTip = "Download saved forms",
                    Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                    ActionLocation = FormBuilderElementProvider.ActionLocation
                }
            });
        }
    }
}