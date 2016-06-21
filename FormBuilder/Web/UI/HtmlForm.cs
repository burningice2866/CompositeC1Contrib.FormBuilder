using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

using Newtonsoft.Json;

using Composite.AspNet.Razor;

using CompositeC1Contrib.FormBuilder.Attributes;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class HtmlForm : IDisposable
    {
        private readonly FormsPage _page;
        private bool _disposed;

        public HtmlForm(FormsPage page, object htmlAttributes)
        {
            _page = page;

            var htmlAttributesDictionary = new Dictionary<string, IList<string>> 
            {
                {
                    "class", new List<string> 
                    {
                        "form",
                        "formbuilder-" + page.Form.Name.ToLowerInvariant()
                    }
                }
            };

            if (page.FormRenderer.Horizontal)
            {
                htmlAttributesDictionary["class"].Add("form-horizontal");
            }

            var htmlElementAttributes = page.Form.Attributes.OfType<HtmlTagAttribute>();
            var action = String.Empty;

            foreach (var attr in htmlElementAttributes)
            {
                if (attr.Attribute == "method")
                {
                    continue;
                }

                if (attr.Attribute == "action")
                {
                    action = attr.Value;

                    continue;
                }

                IList<string> list;
                if (!htmlAttributesDictionary.TryGetValue(attr.Attribute, out list))
                {
                    htmlAttributesDictionary.Add(attr.Attribute, new List<string>());
                }

                htmlAttributesDictionary[attr.Attribute].Add(attr.Value);
            }

            var dictionary = Functions.ObjectToDictionary(htmlAttributes);
            if (dictionary != null)
            {
                if (dictionary.ContainsKey("class"))
                {
                    htmlAttributesDictionary["class"].Add((string)dictionary["class"]);
                }

                if (dictionary.ContainsKey("action"))
                {
                    action = (string)dictionary["action"];
                }
            }

            page.WriteLiteral(String.Format("<form method=\"post\" action=\"{0}\"", action));

            foreach (var kvp in htmlAttributesDictionary)
            {
                page.WriteLiteral(" " + kvp.Key + "=\"");
                foreach (var itm in kvp.Value)
                {
                    page.WriteLiteral(itm + " ");
                }

                page.WriteLiteral("\"");
            }

            if (page.Form.HasFileUpload)
            {
                page.WriteLiteral(" enctype=\"multipart/form-data\"");
            }

            AddRendererSettings();

            page.WriteLiteral(">");
            page.WriteLiteral("<input type=\"hidden\" name=\"__type\" value=\"" + HttpUtility.HtmlAttributeEncode(page.Form.Name) + "\" />");

            foreach (var field in page.Form.Fields.Where(f => f.Label == null))
            {
                AddHiddenField(field.Name, field.Id, field.Value == null ? String.Empty : field.GetValueAsString());
            }

            if (!page.Form.DisableAntiForgery)
            {
                page.WriteLiteral(AntiForgery.GetHtml());
            }
        }

        private void AddRendererSettings()
        {
            var json = JsonConvert.SerializeObject(new
            {
                Type = _page.FormRenderer.GetType().AssemblyQualifiedName,
                Settings = _page.FormRenderer
            });

            _page.WriteLiteral(" data-renderer=\"" + HttpUtility.HtmlAttributeEncode(json) + "\"");
        }

        private void AddHiddenField(string name, string id, string value)
        {
            var s = String.Format("<input type=\"hidden\" name=\"{0}\" id=\"{1}\" value=\"{2}\" />",
                    HttpUtility.HtmlAttributeEncode(name),
                    HttpUtility.HtmlAttributeEncode(id),
                    HttpUtility.HtmlAttributeEncode(value));

            _page.WriteLiteral(s);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _page.WriteLiteral("</form>");

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
