using System;
using System.Xml.Linq;

using Composite.C1Console.Elements;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.FormBuilder.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers
{
    [Serializable]
    public class SaveFormSubmitHandler : FormSubmitHandler, IActionPrivider
    {
        public bool IncludeAttachments { get; set; }

        public override void Submit(IFormModel model)
        {
            SaveFormSubmitFacade.SaveSubmit(model, IncludeAttachments);
        }

        public override void Save(IDynamicFormDefinition definition, XElement handler)
        {
            handler.Add(new XAttribute("IncludeAttachments", IncludeAttachments));
        }

        public void AddActions(IDynamicFormDefinition definition, Element element)
        {
            var downloadActionToken = new DownloadSubmittedFormsActionToken(definition.Name);
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