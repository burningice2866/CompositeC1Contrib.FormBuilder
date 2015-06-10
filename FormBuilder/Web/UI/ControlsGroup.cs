using System;
using System.Text;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class ControlsGroup : IDisposable
    {
        private readonly FormsPage _page;
        private readonly StringBuilder _sb;

        private bool _disposed;

        public ControlsGroup(StringBuilder sb, FormOptions options)
        {
            if (FieldsRow.Current != null)
            {
                return;
            }

            _sb = sb;

            sb.AppendFormat("<div class=\"{0}\">", options.FormRenderer.FieldGroupClass);
        }

        public ControlsGroup(FormsPage page)
        {
            if (FieldsRow.Current != null)
            {
                return;
            }

            _page = page;

            page.WriteLiteral(String.Format("<div class=\"{0}\">", page.Options.FormRenderer.FieldGroupClass));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_page != null)
                {
                    _page.WriteLiteral("</div>");
                }

                if (_sb != null)
                {
                    _sb.Append("</div>");
                }

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
