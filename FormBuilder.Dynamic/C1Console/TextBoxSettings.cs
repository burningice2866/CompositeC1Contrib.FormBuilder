using System;
using System.Collections.Generic;
using System.Linq;

using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console
{
    [Serializable]
    public class TextBoxSettings : IInputTypeSettingsHandler
    {
        public string InputType { get; set; }

        public static Dictionary<string, string> GetInputTypes()
        {
            return new Dictionary<string, string>()
            {
                {"", "Text"},
                {"Email", "Email"},
                {"Date", "Date"}
            };
        }

        public void Load(FormField field)
        {
            InputType = GetInputType(field);
        }

        public void Save(FormField field)
        {
            switch (InputType)
            {
                case "Date":

                    RemoveValidatorAttribute<EmailFieldValidatorAttribute>(field);
                    field.ValueType = typeof (DateTime);

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
                    field.ValueType = typeof (string);

                    break;
            }
        }

        private static string GetInputType(FormField field)
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

        private static void RemoveValidatorAttribute<T>(FormField field) where T : Attribute
        {
            var existingValidator = field.ValidationAttributes.OfType<T>().FirstOrDefault();
            if (existingValidator != null)
            {
                field.Attributes.Remove(existingValidator);
            }
        }
    }
}
