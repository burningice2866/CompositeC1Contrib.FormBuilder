using System;

namespace CompositeC1Contrib.FormBuilder.Web.UI.Rendering
{
    public class Bootstrap3FormRenderer : FormRenderer
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

        public override string HideLabelClass
        {
            get { return "sr-only"; }
        }

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
