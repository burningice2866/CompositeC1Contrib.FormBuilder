using System;

namespace CompositeC1Contrib.FormBuilder.Web.UI.FormRenderers
{
    public class Bootstrap3FormRenderer : FormRendererBase
    {
        public override string ValidationSummaryClass
        {
            get { return "error-notification"; }
        }

        public override string ErrorClass
        {
            get { return "has-error"; }
        }

        public override string ParentGroupClass
        {
            get { return "form"; }
        }

        public override string FieldGroupClass
        {
            get { return String.Empty; }
        }

        public override string HideLabelClass
        {
            get { return "sr-only"; }
        }

        public override string FormControlClass
        {
            get { return "form-control"; }
        }

        public override string FormControlLabelClass(InputElementTypeAttribute inputElement)
        {
            return String.Empty;
        }
    }
}
