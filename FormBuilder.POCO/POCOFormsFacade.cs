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

        public static void MapValues(IPOCOForm instance, FormModel model)
        {
            foreach (var prop in instance.GetType().GetProperties())
            {
                var field = model.Fields.SingleOrDefault(f => f.Name == prop.Name);
                if (field != null && field.ValueType == prop.PropertyType)
                {
                    prop.SetValue(instance, field.Value, null);
                }
            }
        }

        public static void SetDefaultValues(IPOCOForm instance, FormModel model)
        {
            if (instance is IProvidesDefaultValues)
            {
                ((IProvidesDefaultValues)instance).SetDefaultValues();
            }

            foreach (var prop in instance.GetType().GetProperties())
            {
                var field = model.Fields.SingleOrDefault(f => f.Name == prop.Name);
                if (field != null && field.ValueType == prop.PropertyType)
                {
                    field.Value = prop.GetValue(instance, null);
                }
            }
        }
    }
}
