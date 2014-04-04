using System;
using System.Linq;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder
{
    public class POCOFormsFacade
    {
        public static FormModel FromInstance(IPOCOForm instance)
        {
            var formType = instance.GetType();
            var formName = formType.FullName;

            var formNameAttribte = formType.GetCustomAttributes(typeof(FormNameAttribute), false).FirstOrDefault() as FormNameAttribute;
            if (formNameAttribte != null)
            {
                formName = formNameAttribte.FullName;
            }

            var model = new FormModel(formName);

            var validationHandler = instance as IValidationHandler;
            if (validationHandler != null)
            {
                model.OnValidateHandler = validationHandler.OnValidate;
            }

            foreach (var itm in formType.GetCustomAttributes(true).Cast<Attribute>())
            {
                model.Attributes.Add(itm);
            }

            foreach (var prop in formType.GetProperties().Where(p => p.CanRead))
            {
                var attributes = prop.GetCustomAttributes(true).Cast<Attribute>().ToList();
                var field = new FormField(model, prop.Name, prop.PropertyType, attributes)
                {
                    IsReadOnly = !prop.CanWrite
                };

                model.Fields.Add(field);
            }

            return model;
        }

        public static FormModel FromType<T>(T instance) where T : IPOCOForm
        {
            return FromInstance(instance);
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
            var defaultValuesProvider = instance as IProvidesDefaultValues;
            if (defaultValuesProvider != null)
            {
                defaultValuesProvider.SetDefaultValues();
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
