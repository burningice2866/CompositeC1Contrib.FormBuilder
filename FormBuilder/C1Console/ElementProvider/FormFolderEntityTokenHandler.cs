using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;

namespace CompositeC1Contrib.FormBuilder.C1Console.ElementProvider
{
    [Export("FormBuilder", typeof(IElementProviderFor))]
    public class FormFolderEntityTokenHandler : IElementProviderFor
    {
        private ProviderContainer<IElementProviderFor> _entityTokenHandlers;

        public IEnumerable<Type> ProviderFor
        {
            get { return new[] { typeof(FormFolderEntityToken) }; }
        }

        public IEnumerable<Element> Provide(ElementProviderContext context, EntityToken token)
        {
            var folder = (FormFolderEntityToken)token;
            if (folder.FolderType != "Fields")
            {
                yield break;
            }

            var form = ModelsFacade.GetModel(folder.FormName);
            if (form == null)
            {
                yield break;
            }

            if (_entityTokenHandlers == null)
            {
                _entityTokenHandlers = new ProviderContainer<IElementProviderFor>("FormBuilder");
            }

            foreach (var field in form.Fields)
            {
                var elementHandle = context.CreateElementHandle(new FormFieldEntityToken(form.Name, field.Name));
                var fieldElement = new Element(elementHandle)
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = field.Name,
                        ToolTip = field.Name,
                        HasChildren = _entityTokenHandlers.GetProvidersFor(elementHandle.EntityToken).Any(),
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                        OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
                    }
                };

                yield return fieldElement;
            }
        }
    }
}
