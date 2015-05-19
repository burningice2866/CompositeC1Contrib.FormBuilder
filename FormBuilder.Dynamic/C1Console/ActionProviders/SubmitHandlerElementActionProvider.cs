using System;
using System.ComponentModel.Composition;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.FormBuilder.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ActionProviders
{
    [Export(typeof(IElementActionProvider))]
    public class SubmitHandlerElementActionProvider : IElementActionProvider
    {
        public bool IsProviderFor(EntityToken entityToken)
        {
            var token = entityToken as FormSubmitHandlerEntityToken;
            if (token == null)
            {
                return false;
            }

            var type = Type.GetType(token.Type);

            return type != null && type == typeof(SaveFormSubmitHandler);
        }

        public void AddActions(Element element)
        {
            var token = (FormSubmitHandlerEntityToken)element.ElementHandle.EntityToken;

            var downloadActionToken = new DownloadSubmittedFormsActionToken(token.FormName, ".csv");
            element.AddAction(new ElementAction(new ActionHandle(downloadActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Download saved forms",
                    ToolTip = "Download saved forms",
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("generated-type-data-edit"),
                    ActionLocation = FormBuilderElementProvider.ActionLocation
                }
            });
        }
    }
}
