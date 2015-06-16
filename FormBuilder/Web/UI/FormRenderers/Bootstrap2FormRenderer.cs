using System;

namespace CompositeC1Contrib.FormBuilder.Web.UI.FormRenderers
{
    public class Bootstrap2FormRenderer : IFormFormRenderer
    {
        public string ErrorNotificationClass
        {
            get { return "error_notification"; }
        }

        public string ErrorClass
        {
            get { return "error"; }
        }

        public string ParentGroupClass
        {
            get { return "control"; }
        }

        public string FieldGroupClass
        {
            get { return "controls"; }
        }

        public string HideLabelClass
        {
            get { return "hide-text"; }
        }

        public string FormControlClass
        {
            get { return String.Empty; }
        }

        public string FormControlLabelClass(InputElementTypeAttribute inputElement)
        {
            return inputElement.ElementName;
        }
    }
}
