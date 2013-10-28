using System;
using System.Linq;

using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder
{
    public class POCOFormsFacade
    {
        public static FormModel FromInstance(IPOCOForm instance, FormOptions options)
        {
            var formType = instance.GetType();
            var model = new FormModel(formType.FullName);

            if (options != null)
            {
                model.Options = options;
            }

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

        public static FormModel FromType<T>(T instance, FormOptions options) where T : IPOCOForm
        {
            return FromInstance(instance, options);
        }
    }
}
