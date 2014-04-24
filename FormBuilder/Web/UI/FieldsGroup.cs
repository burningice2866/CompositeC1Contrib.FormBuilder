using System;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class FieldsGroup : IDisposable
    {
        private readonly FormsPage _page;
        private bool _disposed;

        public FieldsGroup(FormsPage page, string extraClass)
        {
            _page = page;

            var cssClass = FormRenderer.RendererImplementation.ParentGroupClass + "-group";
            if (!String.IsNullOrEmpty(extraClass))
            {
                cssClass += " " + extraClass;
            }

            page.WriteLiteral(String.Format("<div class=\"{0}\">", cssClass));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _page.WriteLiteral(String.Format("</div>"));

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
