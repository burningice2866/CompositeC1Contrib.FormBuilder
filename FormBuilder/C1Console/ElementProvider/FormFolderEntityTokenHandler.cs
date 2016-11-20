using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

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

            foreach (var field in form.Fields)
            {
                var elementHandle = context.CreateElementHandle(new FormFieldEntityToken(form.Name, field.Name));
                var fieldElement = new Element(elementHandle)
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = field.Name,
                        ToolTip = field.Name,
                        HasChildren = true,
                        Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                        OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
                    }
                };

                yield return fieldElement;
            }
        }
    }
}
