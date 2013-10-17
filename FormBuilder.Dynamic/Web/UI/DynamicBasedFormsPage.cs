using Composite.AspNet.Razor;

using CompositeC1Contrib.FormBuilder.Dynamic;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public abstract class DynamicBasedFormsPage : FormsPage
    {
        protected abstract string FormName { get; }

        public override void ExecutePageHierarchy()
        {
            base.ExecutePageHierarchy();

            if (IsSuccess)
            {
                Write(Html.C1().Markup(SuccessResponse));
            }
            else
            {
                Write(Html.C1().Markup(IntroText));

                Write("<p>Felter med <span class=\"required\">*</span> skal udfyldes.</p>");

                using (BeginForm())
                {
                    Write(WriteErrors());
                    Write(WriteAllFields());

                    WriteLiteral("<div class=\"Buttons\"><input type=\"submit\" value=\"" + SubmitButtonLabel + "\" name=\"SubmitForm\" /></div>");
                }
            }
        }

        public override void Execute() { }

        protected override FormModel ResolveFormModel()
        {
            var model = DynamicFormsFacade.GetFormByName(FormName);

            model.Options = new FormOptions
            {
                HideLabels = false
            };

            return model;
        }
    }
}