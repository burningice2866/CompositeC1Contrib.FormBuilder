using System;

using Composite.Core.Caching;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class FieldsRow : IDisposable
    {
        private const string Key = "FormBuilder:FieldRow";

        private readonly FormsPage _page;
        private bool _disposed;

        public bool IncludeLabels { get; private set; }

        public static FieldsRow Current
        {
            get { return RequestLifetimeCache.TryGet<FieldsRow>(Key); }
        }

        public FieldsRow(FormsPage page, bool includeLabels)
        {
            if (Current != null)
            {
                throw new InvalidOperationException("Fieldrows cannot be nested");
            }

            RequestLifetimeCache.Add(Key, this);

            _page = page;
            IncludeLabels = includeLabels;

            page.WriteLiteral(String.Format("<div class=\"{0} controls-row\">", page.Options.FormRenderer.FieldGroupClass));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _page.WriteLiteral(String.Format("</div>"));

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
