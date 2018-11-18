using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Composite.Core;

using CompositeC1Contrib.FormBuilder.Attributes;

using Microsoft.Extensions.DependencyInjection;

namespace CompositeC1Contrib.FormBuilder
{
    public static class POCOModelsFacade
    {
        private static PropertyInfo prop = typeof(ServiceLocator).GetProperty("RequestScopedServiceProvider", BindingFlags.NonPublic | BindingFlags.Static);

        public static IModel FromType(Type formType)
        {
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
                    var serviceProvider = (IServiceProvider)prop.GetValue(null);

                    var instance = ActivatorUtilities.CreateInstance(serviceProvider, formType);

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
            return from p in type.GetProperties()
                   where p.GetCustomAttribute<FieldLabelAttribute>(true) != null
                       || p.GetCustomAttribute<HiddenFieldAttribute>(true) != null
                   select p;
        }
    }
}
