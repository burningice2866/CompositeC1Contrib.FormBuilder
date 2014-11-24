using System.Collections.Generic;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementActionProvider;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.FormBuilder.C1Console.Actions;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens;

namespace CompositeC1Contrib.FormBuilder.C1Console.ActionProviders
{
    [ConfigurationElementType(typeof(NonConfigurableElementActionProvider))]
    public class ExcelActionProvider : IElementActionProvider
    {
        public IEnumerable<ElementAction> GetActions(EntityToken entityToken)
        {
            var handlerToken = entityToken as FormSubmitHandlerEntityToken;
            if (handlerToken == null)
            {
                yield break;
            }

            var downloadActionToken = new DownloadSubmittedFormsActionToken(handlerToken.FormName, ".xlsx");
            var downloadAction = new ElementAction(new ActionHandle(downloadActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Download saved forms as Excel",
                    ToolTip = "Download saved forms as Excel",
                    Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                    ActionLocation = FormBuilderElementProvider.ActionLocation
                }
            };

            yield return downloadAction;
        }
    }
}