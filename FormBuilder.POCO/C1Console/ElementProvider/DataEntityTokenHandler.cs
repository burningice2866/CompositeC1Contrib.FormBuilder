using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;
using Composite.Data;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Data.Types;

namespace CompositeC1Contrib.FormBuilder.C1Console.ElementProvider
{
    [Export("FormBuilder", typeof(IElementProviderFor))]
    public class DataEntityTokenHandler : IElementProviderFor
    {
        public IEnumerable<Type> ProviderFor => new[] { typeof(DataEntityToken) };

        public IEnumerable<Element> Provide(ElementProviderContext context, EntityToken token)
        {
            var dataToken = (DataEntityToken)token;

            if (!(dataToken.Data is IModelReference modelReference))
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
