using System;

namespace CompositeC1Contrib.FormBuilder.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class FormValidationAttribute : Attribute
    {
        private readonly string _validationMessage;

        protected FormValidationAttribute() { }

        protected FormValidationAttribute(string validationMessage)
        {
            _validationMessage = validationMessage;
        }

        public string GetValidationMessage(FormFieldModel field)
        {
            return Localization.EvaluateT(field, "Validation." + GetType().Name, _validationMessage);
        }

        public virtual FormValidationRule CreateRule(FormField field)
        {
            return CreateRule(field, () => true);
        }

        protected FormValidationRule CreateRule(FormField field, Func<bool> rule)
        {
            return CreateRule(field, new[] { field.Name }, rule);
        }

        protected FormValidationRule CreateRule(FormField field, string[] affectedFields, Func<bool> rule)
        {
            return new FormValidationRule(affectedFields, GetValidationMessage(field.Model), rule);
        }
    }
}
