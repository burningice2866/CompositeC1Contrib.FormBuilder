using System;

namespace CompositeC1Contrib.FormBuilder.Web.UI.Rendering
{
    public class Bootstrap3FormRenderer : FormRenderer
    {
        public override string ValidationSummaryClass => "error-notification";

        public override string ErrorClass => "has-error";

        public override string ParentGroupClass => "form";

        public override string FieldGroupClass
        {
            get
            {
                if (Horizontal)
                {
                    var width = 12 - LabelWidth;

                    return "col-sm-" + width;
                }

                return String.Empty;
            }
        }

        public override string HideLabelClass => "sr-only";

        public override string FormLabelClass
        {
            get
            {
                var cssClass = "control-label";

                if (Horizontal)
                {
                    cssClass += " col-sm-" + LabelWidth;
                }

                return cssClass;
            }
        }

        public override string FormControlClass => "form-control";

        public override string FormControlLabelClass(InputElementTypeAttribute inputElement)
        {
            return String.Empty;
        }
    }
}
