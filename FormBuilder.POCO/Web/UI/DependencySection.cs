using System;
using System.Linq.Expressions;
using System.Web.Mvc;

using Composite.Core.Caching;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class DependencySection<T> : IDisposable where T : class, IPOCOForm
    {
        private const string Key = "FormBuilder:DependencySection";

        private readonly POCOBasedFormsPage<T> _page;
        private bool _disposed;

        public static DependencySection<T> Current
        {
            get { return RequestLifetimeCache.TryGet<DependencySection<T>>(Key); }
        }

        public DependencySection(POCOBasedFormsPage<T> page, Expression<Func<T, object>> fieldSelector, string cssClass)
        {
            if (Current != null)
            {
                throw new InvalidOperationException("DependencySections cannot be nested");
            }

            RequestLifetimeCache.Add(Key, this);

            _page = page;

            var field = page.GetField(fieldSelector);
            var tagBuilder = new TagBuilder("div");

            if (!String.IsNullOrEmpty(cssClass))
            {
                tagBuilder.AddCssClass(cssClass);
            }

            FormRenderer.DependencyAttributeFor(field, tagBuilder);

            page.WriteLiteral(tagBuilder.ToString(TagRenderMode.StartTag));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _page.WriteLiteral("</div>");

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
