using System;

namespace CompositeC1Contrib.FormBuilder.Web.UI.FormRenderers
{
    public class Bootstrap3FormRenderer : IFormFormRenderer
    {
        public string ErrorNotificationClass
        {
            get { return "error-notification"; }
        }

        public string ErrorClass
        {
            get { return "has-error"; }
        }

        public string ParentGroupClass
        {
            get { return "form"; }
        }

        public string FieldGroupClass
        {
            get { return String.Empty; }
        }

        public string FormControlClass
        {
            get { return "form-control"; }
        }
        
        public string FormControlLabelClass(InputElementTypeAttribute inputElement)
        {
            return String.Empty;
        }
    }
}
