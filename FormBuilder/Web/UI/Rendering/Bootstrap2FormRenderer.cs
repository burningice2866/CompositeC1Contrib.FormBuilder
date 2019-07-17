using System;

namespace CompositeC1Contrib.FormBuilder.Web.UI.Rendering
{
    public class Bootstrap2FormRenderer : FormRenderer
    {
        public override string ValidationSummaryClass => "error_notification";

        public override string ErrorClass => "error";

        public override string ParentGroupClass => "control";

        public override string FieldGroupClass => "controls";

        public override string HideLabelClass => "hide-text";

        public override string FormLabelClass => "control-label";

        public override string FormControlClass => String.Empty;

        public override string FormControlLabelClass(InputElementTypeAttribute inputElement)
        {
            return inputElement.ElementName;
        }
    }
}
