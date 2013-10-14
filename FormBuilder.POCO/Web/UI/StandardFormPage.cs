using Composite.AspNet.Razor;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public sealed class StandardFormPage<T> : POCOBasedFormsPage<T> where T : IPOCOForm
    {
        public StandardFormPage() { }

        public override void Execute()
        {
            ExecutePageHierarchy();
        }

        public override void ExecutePageHierarchy()
        {
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

                    Write("<div class=\"Buttons\"><input type=\"submit\" value=\"" + SubmitButtonLabel + "\" name=\"SubmitForm\" /></div>");
                }
            }

            base.ExecutePageHierarchy();
        }
    }
}
