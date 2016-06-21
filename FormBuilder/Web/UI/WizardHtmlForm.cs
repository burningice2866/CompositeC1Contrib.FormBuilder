using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

using Composite.AspNet.Razor;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class WizardHtmlForm : IDisposable
    {
        private readonly StandardFormWizardPage _page;
        private bool _disposed;

        public WizardHtmlForm(StandardFormWizardPage page, Wizard wizard, object htmlAttributes)
        {
            _page = page;

            var htmlAttributesDictionary = new Dictionary<string, IList<string>> 
            {
                {
                    "class", new List<string> 
                    {
                        "form",
                        "formwizard-" + wizard.Name.ToLowerInvariant(),
                    }
                }
            };

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

            if (wizard.HasFileUpload)
            {
                page.WriteLiteral(" enctype=\"multipart/form-data\"");
            }

            page.WriteLiteral(" data-renderer=\"" + page.FormRenderer.GetType().AssemblyQualifiedName + "\"");
            page.WriteLiteral(">");

            page.WriteLiteral("<input type=\"hidden\" name=\"__type\" value=\"" + HttpUtility.HtmlAttributeEncode(wizard.Name) + "\" />");

            for (int i = 0; i < wizard.Steps.Count; i++)
            {
                var step = wizard.Steps[i];

                RenderHiddenField("step_" + (i + 1), "step_" + (i + 1), step.FormName);
            }

            foreach (var field in wizard.Fields.Where(f => f.Label == null))
            {
                RenderHiddenField(field.Name, field.Id, field.Value == null ? String.Empty : field.GetValueAsString());
            }

            if (!wizard.DisableAntiForgery)
            {
                page.WriteLiteral(AntiForgery.GetHtml());
            }
        }

        private void RenderHiddenField(string name, string id, string value)
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

        public void EndForm()
        {
            Dispose(true);
        }
    }
}
