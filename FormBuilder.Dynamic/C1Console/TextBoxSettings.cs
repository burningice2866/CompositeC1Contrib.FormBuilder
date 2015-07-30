using System;
using System.Collections.Generic;
using System.Linq;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console
{
    [Serializable]
    public class TextBoxSettings : IInputTypeSettingsHandler
    {
        public string InputType { get; set; }
        public string FormatString { get; set; }

        public static Dictionary<string, string> GetInputTypes()
        {
            return new Dictionary<string, string>()
            {
                {"", "Text"},
                {"Email", "Email"},
                {"Date", "Date"}
            };
        }

        public static Dictionary<string, string> GetFormatTypes()
        {
            var date = new DateTime(1900, 12, 31, 23, 59, 59);
            var formatStrings = new[] { "D", "d" };

            return formatStrings.ToDictionary(s => s, date.ToString);
        }

        public void Load(FormFieldModel field)
        {
            InputType = GetInputType(field);
            FormatString = GetFormatString(field);
        }

        public void Save(FormFieldModel field)
        {
            switch (InputType)
            {
                case "Date":

                    RemoveValidatorAttribute<EmailFieldValidatorAttribute>(field);
                    field.ValueType = typeof(DateTime);

                    break;

                case "Email":

                    field.ValueType = typeof(string);

                    if (!field.ValidationAttributes.OfType<EmailFieldValidatorAttribute>().Any())
                    {
                        field.Attributes.Add(new EmailFieldValidatorAttribute("Email is invalid"));
                    }

                    break;

                default:

                    RemoveValidatorAttribute<EmailFieldValidatorAttribute>(field);
                    field.ValueType = typeof(string);

                    break;
            }

            RemoveAttribute<DisplayFormatAttribute>(field);

            if (!String.IsNullOrEmpty(FormatString))
            {
                var formatAttribute = new DisplayFormatAttribute(FormatString);

                field.Attributes.Add(formatAttribute);
            }
        }

        private static string GetInputType(FormFieldModel field)
        {
            if (field.ValueType == typeof(DateTime))
            {
                return "Date";
            }

            var existingValidator = field.ValidationAttributes.OfType<EmailFieldValidatorAttribute>().FirstOrDefault();
            if (existingValidator != null)
            {
                return "Email";
            }

            return String.Empty;
        }

        private static string GetFormatString(FormFieldModel field)
        {
            var formatAttr = field.Attributes.OfType<DisplayFormatAttribute>().SingleOrDefault();

            return formatAttr == null ? String.Empty : formatAttr.FormatString;
        }

        private static void RemoveValidatorAttribute<T>(FormFieldModel field) where T : Attribute
        {
            var existingValidator = field.ValidationAttributes.OfType<T>().FirstOrDefault();
            if (existingValidator != null)
            {
                field.Attributes.Remove(existingValidator);
            }
        }

        private static void RemoveAttribute<T>(FormFieldModel field) where T : Attribute
        {
            var existingAttribute = field.Attributes.OfType<T>().FirstOrDefault();
            if (existingAttribute != null)
            {
                field.Attributes.Remove(existingAttribute);
            }
        }
    }
}
