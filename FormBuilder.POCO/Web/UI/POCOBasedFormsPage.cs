using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

using Composite.AspNet.Razor;

using CompositeC1Contrib.FormBuilder.FunctionProviders;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public abstract class POCOBasedFormsPage<T> : FormsPage where T : class, IPOCOForm
    {
        protected new T Form { get; }

        protected override FormRequestContext RequestContext { get; }

        protected POCOBasedFormsPage()
        {
            var model = POCOModelsFacade.FromType(typeof(T));
            var context = new POCOFormBuilderRequestContext(model);

            RequestContext = context;
            Form = (T)context.Instance;
        }

        public override void ExecutePageHierarchy()
        {
            FunctionContextContainer.AddParameter(BaseFormFunction.RequestContextKey, RequestContext);
            FunctionContextContainer.AddParameter(BaseFormFunction.InstanceKey, RequestContext.ModelInstance);

            RequestContext.Execute(Context);

            base.ExecutePageHierarchy();
        }

        protected DependencySection<T> BeginDependencySection(Expression<Func<T, object>> fieldSelector)
        {
            return new DependencySection<T>(this, fieldSelector, null);
        }

        protected DependencySection<T> BeginDependencySection(Expression<Func<T, object>> fieldSelector, string cssClass)
        {
            return new DependencySection<T>(this, fieldSelector, cssClass);
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
            var field = RequestContext.ModelInstance.Fields.Get(prop.Name);

            return FormRenderer.NameFor(field);
        }

        public FormField GetField(Expression<Func<T, object>> fieldSelector)
        {
            var prop = GetProperty(fieldSelector);

            return RequestContext.ModelInstance.Fields.Get(prop.Name);
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
