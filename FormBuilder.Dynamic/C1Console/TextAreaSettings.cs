using System;
using System.Linq;

using CompositeC1Contrib.FormBuilder.Validation;
using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console
{
    [Serializable]
    public class TextAreaSettings : IInputTypeSettingsHandler
    {
        public int MaxLength { get; set; }
        public int Cols { get; set; }
        public int Rows { get; set; }

        public void Load(FormFieldModel field)
        {
            var existingValidator = field.ValidationAttributes.OfType<MaxFieldLengthAttribute>().FirstOrDefault();
            if (existingValidator != null)
            {
                MaxLength = existingValidator.Length;
            }

            var inputElement = field.InputElementType as TextAreaInputElementAttribute;
            if (inputElement != null)
            {
                Cols = inputElement.Cols;
                Rows = inputElement.Rows;
            }
        }

        public void Save(FormFieldModel field)
        {
            var existingValidator = field.ValidationAttributes.OfType<MaxFieldLengthAttribute>().FirstOrDefault();
            if (existingValidator != null)
            {
                field.Attributes.Remove(existingValidator);
            }

            if (MaxLength > 0)
            {
                var maxLengthValidator = new MaxFieldLengthAttribute("Max length is " + MaxLength, MaxLength);
                
                field.Attributes.Add(maxLengthValidator);
            }

            var inputTypeAttribute = field.Attributes.OfType<InputElementTypeAttribute>().FirstOrDefault();
            if (inputTypeAttribute != null)
            {
                field.Attributes.Remove(inputTypeAttribute);
            }

            inputTypeAttribute = new TextAreaInputElementAttribute(Cols, Rows);
            field.Attributes.Add(inputTypeAttribute);
        }
    }
}
