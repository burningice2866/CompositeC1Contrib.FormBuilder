using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public abstract class POCOBasedFormsPage<T> : FormsPage where T : IPOCOForm
    {
        protected T Form { get; private set; }

        public POCOBasedFormsPage()
        {
            Form = Activator.CreateInstance<T>();
            RenderingModel = FromBaseForm<T>(Form, Options);
        }

        public override void ExecutePageHierarchy()
        {
            if (!IsOwnSubmit)
            {
                if (Form is IProvidesDefaultValues)
                {
                    ((IProvidesDefaultValues)Form).SetDefaultValues();
                }

                foreach (var prop in typeof(T).GetProperties())
                {
                    var field = RenderingModel.Fields.SingleOrDefault(f => f.Name == prop.Name);
                    if (field != null && field.ValueType == prop.PropertyType)
                    {
                        field.Value = prop.GetValue(Form, null);
                    }
                }
            }

            base.ExecutePageHierarchy();            
        }

        protected override void OnMappedValues()
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                var field = RenderingModel.Fields.SingleOrDefault(f => f.Name == prop.Name);
                if (field != null && field.ValueType == prop.PropertyType)
                {
                    prop.SetValue(Form, field.Value, null);
                }
            }
        }

        protected IHtmlString DependencyAttributeFor(Expression<Func<T, object>> fieldSelector)
        {
            var sb = new StringBuilder();
            var prop = GetProperty(fieldSelector);
            var field = RenderingModel.Fields.Single(f => f.Name == prop.Name);            

            FormRenderer.DependencyAttributeFor(field, sb);

            return new HtmlString(sb.ToString().Trim());
        }

        protected IHtmlString FieldFor(Expression<Func<T, object>> fieldSelector)
        {
            var prop = GetProperty(fieldSelector);
            var field = RenderingModel.Fields.Single(f => f.Name == prop.Name);

            return FormRenderer.FieldFor(field);
        }

        protected IHtmlString NameFor(Expression<Func<T, object>> fieldSelector)
        {
            var prop = GetProperty(fieldSelector);
            var field = RenderingModel.Fields.Single(f => f.Name == prop.Name);

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

        private FormModel FromBaseForm<T>(T instance, FormOptions options) where T : IPOCOForm
        {
            var formType = typeof(T);

            var model = new FormModel
            {
                Name = formType.FullName,
                Options = options,
                OnSubmitHandler = instance.Submit
            };

            if (instance is IValidationHandler)
            {
                model.OnValidateHandler = ((IValidationHandler)instance).OnValidate;
            }

            foreach (var itm in formType.GetCustomAttributes(true).Cast<Attribute>())
            {
                model.Attributes.Add(itm);
            }

            foreach (var prop in formType.GetProperties().Where(p => p.CanRead && p.CanWrite))
            {
                var attributes = prop.GetCustomAttributes(true).Cast<Attribute>().ToList();
                var field = new FormField(model, prop.Name, prop.PropertyType, attributes);

                model.Fields.Add(field);
            }

            return model;
        }
    }
}
