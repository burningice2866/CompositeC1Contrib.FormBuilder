using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;

using Composite.AspNet.Razor;
using Composite.Functions;

using CompositeC1Contrib.FormBuilder.FunctionProviders;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public abstract class POCOBasedFormsPage<T> : FormsPage where T : class, IPOCOForm
    {
        protected new T Form { get; private set; }

        private FormRequestContext _requestContext;
        protected override FormRequestContext RequestContext
        {
            get { return _requestContext; }
        }

        protected POCOBasedFormsPage()
        {
            var model = POCOModelsFacade.FromType(typeof(T));
            var context = new POCOFormBuilderRequestContext(model.Name);

            _requestContext = context;
            Form = (T)context.Instance;
        }

        public override void ExecutePageHierarchy()
        {
            var functionContext = new FunctionContextContainer(FunctionContextContainer, new Dictionary<string, object>
            {
                { BaseFormFunction.RequestContextKey, RequestContext },
                { BaseFormFunction.InstanceKey, RequestContext.ModelInstance }
            });

            var functionContextField = typeof(RazorHelper).GetField("PageContext_FunctionContextContainer", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

            PageData[functionContextField] = functionContext;

            RequestContext.Execute(Context);

            base.ExecutePageHierarchy();
        }

        protected IHtmlString DependencyAttributeFor(Expression<Func<T, object>> fieldSelector)
        {
            var sb = new StringBuilder();
            var field = GetField(fieldSelector);

            FormRenderer.DependencyAttributeFor(field, sb);

            return new HtmlString(sb.ToString().Trim());
        }

        protected IHtmlString InputFor(Expression<Func<T, object>> fieldSelector)
        {
            return InputFor(fieldSelector, new { });
        }

        protected IHtmlString InputFor(Expression<Func<T, object>> fieldSelector, object htmlAttributes)
        {
            var field = GetField(fieldSelector);
            var dictionary = Functions.ObjectToDictionary(htmlAttributes).ToDictionary(t => t.Key, t => t.Value.ToString());

            return FormRenderer.InputFor(RequestContext, field, dictionary);
        }

        protected IHtmlString FieldFor(Expression<Func<T, object>> fieldSelector)
        {
            return FieldFor(fieldSelector, new { });
        }

        protected IHtmlString FieldFor(Expression<Func<T, object>> fieldSelector, object htmlAttributes)
        {
            var field = GetField(fieldSelector);
            var dictionary = Functions.ObjectToDictionary(htmlAttributes).ToDictionary(t => t.Key, t => t.Value.ToString());

            return FormRenderer.FieldFor(RequestContext, field, dictionary);
        }

        protected IHtmlString NameFor(Expression<Func<T, object>> fieldSelector)
        {
            var prop = GetProperty(fieldSelector);
            var field = RequestContext.ModelInstance.Fields.Single(f => f.Name == prop.Name);

            return FormRenderer.NameFor(field);
        }

        public FormField GetField(Expression<Func<T, object>> fieldSelector)
        {
            var prop = GetProperty(fieldSelector);

            return RequestContext.ModelInstance.Fields.Single(f => f.Name == prop.Name);
        }

        public PropertyInfo GetProperty(string name)
        {
            return GetType().GetProperty(name);
        }

        public PropertyInfo GetProperty(Expression<Func<T, object>> fieldSelector)
        {
            MemberExpression memberExpression;

            switch (fieldSelector.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var unaryExpression = (UnaryExpression)fieldSelector.Body;
                    memberExpression = (MemberExpression)unaryExpression.Operand;

                    break;

                default:
                    memberExpression = (MemberExpression)fieldSelector.Body;

                    break;
            }

            return (PropertyInfo)memberExpression.Member;
        }
    }
}
