using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CompositeC1Contrib.FormBuilder.Validation;
using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    [Serializable]
    public class TextBoxSettings : IInputTypeSettingsHandler
    {
        public string InputType { get; set; }

        public static Dictionary<string, string> GetInputTypes()
        {
            return new Dictionary<string, string>()
            {
                {"Text", "Text"},
                {"Email", "Email"},
                {"Date", "Date"}
            };
        }

        public void Load(FormField field)
        {
            if (field.ValueType == typeof (DateTime))
            {
                InputType = "Date";
            }

            var existingValidator = field.ValidationAttributes.OfType<EmailFieldValidatorAttribute>().FirstOrDefault();
            if (existingValidator != null)
            {
                InputType = "Email";
            }
        }

        public void Save(FormField field)
        {
            switch (InputType)
            {
                case "Date":
                    field.ValueType = typeof (DateTime);

                    break;
                case "Email":
                    field.ValueType = typeof(string);
                    var existingValidator = field.ValidationAttributes.OfType<EmailFieldValidatorAttribute>().FirstOrDefault();
                    if (existingValidator != null)
                    {
                        field.Attributes.Remove(existingValidator);
                    }
                    field.Attributes.Add(new EmailFieldValidatorAttribute("Email is invalid"));

                    break;
                default:
                    field.ValueType = typeof (string);

                    break;
            }
        }
    }
}
