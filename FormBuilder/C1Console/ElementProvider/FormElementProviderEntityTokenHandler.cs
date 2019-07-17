using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;
using Composite.Data;
using Composite.Data.Types;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Data.Types;

namespace CompositeC1Contrib.FormBuilder.C1Console.ElementProvider
{
    [Export("FormBuilder", typeof(IElementProviderFor))]
    public class FormElementProviderEntityTokenHandler : IElementProviderFor
    {
        public IEnumerable<Type> ProviderFor => new[] { typeof(FormElementProviderEntityToken) };

        public IEnumerable<Element> Provide(ElementProviderContext context, EntityToken token)
        {
            return GetNamespaceAndFormElements(context, String.Empty);
        }

        public static IEnumerable<Element> GetNamespaceAndFormElements(ElementProviderContext context, string ns)
        {
            using (var data = new DataConnection())
            {
                var allModelReferences = data.Get<IModelReference>();

                var folders = new List<string>();
                var formElements = new List<Element>();

                if (String.IsNullOrEmpty(ns))
                {
                    var homepageIds = data.Get<IPageStructure>().Where(s => s.ParentId == Guid.Empty).Select(s => s.Id);
                    if (homepageIds.Count() > 1)
                    {
                        folders.AddRange(homepageIds
                            .Select(id => PageManager.GetPageById(id))
                            .Select(page => SanatizeFormName(page.Title)));
                    }
                }
                else
                {
                    allModelReferences = allModelReferences.Where(def => def.Name.StartsWith(ns + "."));
                }

                foreach (var modelReference in allModelReferences)
                {
                    var label = modelReference.Name;

                    if (!String.IsNullOrEmpty(ns))
                    {
                        label = label.Remove(0, ns.Length + 1);
                    }

                    var split = label.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length > 1)
                    {
                        var folder = split[0];

                        if (!folders.Contains(folder))
                        {
                            folders.Add(folder);
                        }
                    }
                    else if (split.Length == 1)
                    {
                        var token = modelReference.GetDataEntityToken();

                        var elementHandle = context.CreateElementHandle(token);
                        var formElement = new Element(elementHandle)
                        {
                            VisualData = new ElementVisualizedData
                            {
                                Label = label,
                                ToolTip = label,
                                HasChildren = true,
                                Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                                OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
                            }
                        };

                        formElements.Add(formElement);
                    }
                }

                foreach (var folder in folders.OrderBy(f => f))
                {
                    var handleNamespace = folder;
                    if (!String.IsNullOrEmpty(ns))
                    {
                        handleNamespace = ns + "." + handleNamespace;
                    }

                    var folderElement = NamespaceFolderEntityToken.CreateElement(context, typeof(FormBuilderElementProvider).Name, folder, handleNamespace);

                    yield return folderElement;
                }

                foreach (var form in formElements)
                {
                    yield return form;
                }
            }
        }

        private static string SanatizeFormName(string input)
        {
            var replacementChars = new Dictionary<string, string>
            {
                {"Æ", "Ae" },
                {"æ", "ae"},
                {"Ø", "Oe"},
                {"ø", "oe"},
                {"Å", "Aa"},
                {"å", "aa"},
                {"²", "2"},
                {"&", "og"}
            };

            var illegalChars = new[] { ".", "-", "(", ")", ",", "/", "\"", "§", ":", "?", "+", "Ë", "é" };

            input = illegalChars.Aggregate(input, (current, s) => current.Replace(s, String.Empty));
            input = replacementChars.Aggregate(input, (current, s) => current.Replace(s.Key, s.Value));

            var split = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < split.Length; i++)
            {
                var el = split[i];

                split[i] = el.Substring(0, 1).ToUpper();

                if (el.Length > 1)
                {
                    split[i] += el.Substring(1, el.Length - 1);
                }
            }

            return String.Join(String.Empty, split);
        }
    }
}
