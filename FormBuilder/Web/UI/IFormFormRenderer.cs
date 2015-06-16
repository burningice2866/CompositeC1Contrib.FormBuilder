namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public interface IFormFormRenderer
    {
        string ErrorNotificationClass { get; }
        string ErrorClass { get; }
        string HideLabelClass { get; }
        string ParentGroupClass { get; }
        string FieldGroupClass { get; }
        string FormControlClass { get; }

        string FormControlLabelClass(InputElementTypeAttribute inputElement);
    }
}
