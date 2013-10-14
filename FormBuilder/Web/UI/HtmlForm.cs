using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Composite.AspNet.Razor;

using CompositeC1Contrib.FormBuilder.Attributes;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class HtmlForm : IDisposable
    {
        private FormsPage _page;
        private FormModel _model;
        private bool _disposed;

        public HtmlForm(FormsPage page, FormModel model, object htmlAttributes)
        {
            _page = page;
            _model = model;

            var htmlAttributesDictionary = new Dictionary<string, IList<string>>();

            htmlAttributesDictionary.Add("class", new List<string>());

            htmlAttributesDictionary["class"].Add("form");
            htmlAttributesDictionary["class"].Add("formbuilder-" + _model.Name.ToLowerInvariant());

            var htmlElementAttributes = _model.Attributes.OfType<HtmlTagAttribute>();
            var action = String.Empty;

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

            foreach (var attr in htmlElementAttributes)
            {
                IList<string> list;
                if (!htmlAttributesDictionary.TryGetValue(attr.Attribute, out list))
                {
                    htmlAttributesDictionary.Add(attr.Attribute, new List<string>());
                }

                htmlAttributesDictionary[attr.Attribute].Add(attr.Value);
            }

            page.WriteLiteral(String.Format("<form method=\"post\" action=\"{1}\"", _model.Name, action));

            foreach (var kvp in htmlAttributesDictionary)
            {
                page.WriteLiteral(" " + kvp.Key + "=\"");
                foreach (var itm in kvp.Value)
                {
                    page.WriteLiteral(itm + " ");
                }

                page.WriteLiteral("\"");
            }

            if (_model.HasFileUpload)
            {
                page.WriteLiteral(" enctype=\"multipart/form-data\"");
            }

            page.WriteLiteral(">");

            page.WriteLiteral("<input type=\"hidden\" name=\"__type\" value=\"" + HttpUtility.HtmlAttributeEncode(_model.Name) + "\" />");

            foreach (var field in _model.Fields.Where(f => f.Label == null))
            {
                var s = String.Format("<input type=\"hidden\" name=\"{0}\" id=\"{1}\" value=\"{2}\" />",
                    HttpUtility.HtmlAttributeEncode(field.Name),
                    HttpUtility.HtmlAttributeEncode(field.Id),
                    field.Value == null ? String.Empty : HttpUtility.HtmlAttributeEncode(FormRenderer.GetValue(field)));

                page.WriteLiteral(s);
            }
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

        public void EndForm()
        {
            Dispose(true);
        }
    }
}
