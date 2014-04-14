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
    public abstract class POCOBasedFormsPage<T> : FormsPage where T : IPOCOForm
    {
        private readonly T _form = Activator.CreateInstance<T>();
        protected T Form
        {
            get { return _form; }
        }

        private FormBuilderRequestContext _context;
        public override FormBuilderRequestContext RenderingContext
        {
            get
            {
                if (_context == null)
                {
                    var model = POCOFormsFacade.FromType(Form);

                    _context = new POCOFormBuilderRequestContext(model.Name, Form);
                    _context.Execute();
                }

                return _context;
            }
        }

        public override void ExecutePageHierarchy()
        {
            var functionContext = new FunctionContextContainer(FunctionContextContainer, new Dictionary<string, object>
            {
                { StandardFormFunction.RenderingContextKey, RenderingContext },
                { StandardFormFunction.FormModelKey, RenderingContext.RenderingModel }
            });

            var functionContextField = typeof(RazorHelper).GetField("PageContext_FunctionContextContainer", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

            PageData[functionContextField] = functionContext;

            base.ExecutePageHierarchy();
        }

        protected IHtmlString DependencyAttributeFor(Expression<Func<T, object>> fieldSelector)
        {
            var sb = new StringBuilder();
            var prop = GetProperty(fieldSelector);
            var field = RenderingContext.RenderingModel.Fields.Single(f => f.Name == prop.Name);

            FormRenderer.DependencyAttributeFor(field, sb);

            return new HtmlString(sb.ToString().Trim());
        }

        protected IHtmlString FieldFor(Expression<Func<T, object>> fieldSelector)
        {
            return FieldFor(fieldSelector, new {});
        }

        protected IHtmlString FieldFor(Expression<Func<T, object>> fieldSelector, object htmlAttributes)
        {
            var prop = GetProperty(fieldSelector);
            var field = RenderingContext.RenderingModel.Fields.Single(f => f.Name == prop.Name);
            var dictionary = Functions.ObjectToDictionary(htmlAttributes).ToDictionary(t => t.Key, t => t.Value.ToString());

            return FormRenderer.FieldFor(Options, field, dictionary);
        }

        protected IHtmlString NameFor(Expression<Func<T, object>> fieldSelector)
        {
            var prop = GetProperty(fieldSelector);
            var field = RenderingContext.RenderingModel.Fields.Single(f => f.Name == prop.Name);

            return FormRenderer.NameFor(this, field);
        }

        public PropertyInfo GetProperty(string name)
        {
            return GetType().GetProperty(name);
        }

        public PropertyInfo GetProperty(Expression<Func<T, object>> field)
        {
            MemberExpression memberExpression;

            switch (field.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var unaryExpression = (UnaryExpression)field.Body;
                    memberExpression = (MemberExpression)unaryExpression.Operand;

                    break;

                default:
                    memberExpression = (MemberExpression)field.Body;

                    break;
            }

            return (PropertyInfo)memberExpression.Member;
        }
    }
}
