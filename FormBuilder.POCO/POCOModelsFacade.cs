using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CompositeC1Contrib.FormBuilder
{
    public class POCOModelsFacade
    {
        public static IModel FromType(Type formType)
        {
            ConstructorInfo constructor = null;

            foreach (var ctor in formType.GetConstructors(BindingFlags.Instance | BindingFlags.Public))
            {
                var parameters = ctor.GetParameters();

                if (parameters.Length == 0)
                {
                    constructor = ctor;
                }
            }

            if (constructor == null)
            {
                throw new InvalidOperationException("No parameterless constructor on form");
            }

            var formName = formType.FullName;

            var formNameAttribte = formType.GetCustomAttributes(typeof(FormNameAttribute), false).FirstOrDefault() as FormNameAttribute;
            if (formNameAttribte != null)
            {
                formName = formNameAttribte.FullName;
            }

            var model = new FormModel(formName)
            {
                Constructor = f =>
                {
                    var instance = constructor.Invoke(null);

                    f.FormData.Add("PocoInstance", instance);
                }
            };

            if (typeof(IValidationHandler).IsAssignableFrom(formType))
            {
                model.OnValidateHandler = (f, e) =>
                {
                    var instance = (IValidationHandler)f.FormData["PocoInstance"];

                    instance.OnValidate(e);
                };
            }

            model.SetDefaultValuesHandler = f =>
            {
                var instance = (IPOCOForm)f.FormData["PocoInstance"];

                SetDefaultValues(instance, f);
            };

            foreach (var itm in formType.GetCustomAttributes(true).Cast<Attribute>())
            {
                model.Attributes.Add(itm);
            }

            foreach (var prop in GetFieldProps(formType).Where(p => p.CanRead))
            {
                var attributes = prop.GetCustomAttributes(true).Cast<Attribute>().ToList();
                var field = new FormFieldModel(model, prop.Name, prop.PropertyType, attributes)
                {
                    IsReadOnly = !prop.CanWrite
                };

                model.Fields.Add(field);
            }

            return model;
        }

        public static void MapValues(IPOCOForm instance, Form form)
        {
            foreach (var prop in GetFieldProps(instance).Where(p => p.CanWrite))
            {
                var field = form.Fields.SingleOrDefault(f => f.Name == prop.Name);
                if (field != null && field.ValueType == prop.PropertyType)
                {
                    prop.SetValue(instance, field.Value, null);
                }
            }
        }

        public static void SetDefaultValues(IPOCOForm instance, Form form)
        {
            var defaultValuesProvider = instance as IProvidesDefaultValues;
            if (defaultValuesProvider != null)
            {
                defaultValuesProvider.SetDefaultValues();
            }

            foreach (var prop in GetFieldProps(instance).Where(p => p.CanRead))
            {
                var field = form.Fields.SingleOrDefault(f => f.Name == prop.Name);
                if (field != null && field.ValueType == prop.PropertyType)
                {
                    field.Value = prop.GetValue(instance, null);
                }
            }
        }

        private static IEnumerable<PropertyInfo> GetFieldProps(IPOCOForm instance)
        {
            return GetFieldProps(instance.GetType());
        }

        private static IEnumerable<PropertyInfo> GetFieldProps(Type type)
        {
            return type.GetProperties().Where(p => p.GetCustomAttribute<ExcludeFieldAttribute>(true) == null);
        }
    }
}
