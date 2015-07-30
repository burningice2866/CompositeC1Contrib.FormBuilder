using System.Collections.Generic;
using System.ComponentModel.Composition;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;
using Composite.Data;

using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Data.Types;

namespace CompositeC1Contrib.FormBuilder.C1Console.ElementProvider
{
    [Export(typeof(IEntityTokenBasedElementProvider))]
    public class DataEntityTokenHandler : IEntityTokenBasedElementProvider
    {
        public bool IsProviderFor(EntityToken token)
        {
            return token is DataEntityToken;
        }

        public IEnumerable<Element> Handle(ElementProviderContext context, EntityToken token)
        {
            var modelReference = (IModelReference)((DataEntityToken)token).Data;
            if (modelReference == null)
            {
                yield break;
            }

            var dataId = (ModelReferenceId)modelReference.DataSourceId.DataId;
            if (dataId.Source != typeof(POCOModelsProvider).Name)
            {
                yield break;
            }

            var fieldsElementHandle = context.CreateElementHandle(new FormFolderEntityToken(modelReference.Name, "Fields"));
            var fieldsElement = new Element(fieldsElementHandle)
            {
                VisualData = new ElementVisualizedData
                {
                    Label = "Fields",
                    ToolTip = "Fields",
                    HasChildren = true,
                    Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                    OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
                }
            };

            yield return fieldsElement;
        }
    }
}
