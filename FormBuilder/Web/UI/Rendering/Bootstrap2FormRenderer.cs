using System;

namespace CompositeC1Contrib.FormBuilder.Web.UI.Rendering
{
    public class Bootstrap2FormRenderer : FormRenderer
    {
        public override string ValidationSummaryClass
        {
            get { return "error_notification"; }
        }

        public override string ErrorClass
        {
            get { return "error"; }
        }

        public override string ParentGroupClass
        {
            get { return "control"; }
        }

        public override string FieldGroupClass
        {
            get { return "controls"; }
        }

        public override string HideLabelClass
        {
            get { return "hide-text"; }
        }

        public override string FormLabelClass
        {
            get { return "control-label"; }
        }

        public override string FormControlClass
        {
            get { return String.Empty; }
        }

        public override string FormControlLabelClass(InputElementTypeAttribute inputElement)
        {
            return inputElement.ElementName;
        }
    }
}
