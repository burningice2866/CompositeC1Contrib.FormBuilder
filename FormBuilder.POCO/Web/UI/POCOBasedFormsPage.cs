using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;

using Composite.AspNet.Razor;
using Composite.Functions;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public abstract class POCOBasedFormsPage<T> : FormsPage where T : IPOCOForm
    {
        protected T _form = Activator.CreateInstance<T>();
        protected T Form
        {
            get { return _form; }
        }

        private FormBuilderRequestContext _context;
        protected override FormBuilderRequestContext RenderingContext
        {
            get
            {
                if (_context == null)
                {
                    var model = POCOFormsFacade.FromType<T>(Form, Options);

                    _context = FormBuilderRequestContext.Setup(model, 
                        (m) => POCOFormsFacade.MapValues(Form, m), 
                        Form.Submit,
                        (m) => POCOFormsFacade.SetDefaultValues(Form, m)
                    );
                }

                return _context;
            }
        }

        public override void ExecutePageHierarchy()
        {
            var functionContext = new FunctionContextContainer(FunctionContextContainer, new Dictionary<string, object>
            {
                { "RenderingContext", RenderingContext },
                { "FormModel", RenderingContext.RenderingModel }
            });

            var functionContext_field = typeof(RazorHelper).GetField("PageContext_FunctionContextContainer", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

            PageData[functionContext_field] = functionContext;

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
            var prop = GetProperty(fieldSelector);
            var field = RenderingContext.RenderingModel.Fields.Single(f => f.Name == prop.Name);

            return FormRenderer.FieldFor(field);
        }

        protected IHtmlString NameFor(Expression<Func<T, object>> fieldSelector)
        {
            var prop = GetProperty(fieldSelector);
            var field = RenderingContext.RenderingModel.Fields.Single(f => f.Name == prop.Name);

            return FormRenderer.NameFor(field);
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
