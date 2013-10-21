using CompositeC1Contrib.FormBuilder.Dynamic;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public abstract class DynamicBasedFormsPage : FormsPage
    {
        public string FormName { get; set; }

        protected override FormModel ResolveFormModel()
        {
            var definition = DynamicFormsFacade.GetFormByName(FormName);

            definition.Model.Options = new FormOptions
            {
                HideLabels = false
            };

            return definition.Model;
        }
    }
}