using System;
using System.IO;
using System.Text;

using CompositeC1Contrib.FormBuilder.Web.UI.Rendering;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class ControlsGroup : IDisposable
    {
        private readonly TextWriter _output;
        private readonly FormRenderer _renderer;

        private bool _disposed;

        public ControlsGroup(StringBuilder sb, FormRenderer renderer) : this(new StringWriter(sb), renderer) { }
        public ControlsGroup(FormsPage page) : this(page.Output, page.FormRenderer) { }

        private ControlsGroup(TextWriter output, FormRenderer renderer)
        {
            if (FieldsRow.Current != null)
            {
                return;
            }

            _output = output;
            _renderer = renderer;

            if (!String.IsNullOrEmpty(_renderer.FieldGroupClass))
            {
                _output.Write($"<div class=\"{_renderer.FieldGroupClass}\">");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (!String.IsNullOrEmpty(_renderer.FieldGroupClass))
                {
                    _output.Write("</div>");
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
