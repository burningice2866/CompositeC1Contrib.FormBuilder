using System;
using System.IO;
using System.Text;

using Composite.Core.Caching;

using CompositeC1Contrib.FormBuilder.Web.UI.Rendering;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class FieldsRow : IDisposable
    {
        private const string Key = "FormBuilder:FieldRow";

        private readonly TextWriter _output;
        private readonly FormRenderer _renderer;

        private bool _disposed;

        public static FieldsRow Current
        {
            get { return RequestLifetimeCache.TryGet<FieldsRow>(Key); }
        }

        public FieldsRow(StringBuilder sb, FormRenderer renderer) : this(new StringWriter(sb), renderer) { }
        public FieldsRow(FormsPage page, FormRenderer renderer) : this(page.Output, renderer) { }

        public FieldsRow(TextWriter output, FormRenderer renderer)
        {
            if (Current != null)
            {
                throw new InvalidOperationException("Fieldrows cannot be nested");
            }

            RequestLifetimeCache.Add(Key, this);

            _output = output;
            _renderer = renderer;

            _output.Write(String.Format("<div class=\"{0} controls-row\">", _renderer.FieldGroupClass));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _output.Write(String.Format("</div>"));

                RequestLifetimeCache.Remove(Key);

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
