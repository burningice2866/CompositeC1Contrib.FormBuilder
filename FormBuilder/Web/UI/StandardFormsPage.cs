
namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public abstract class StandardFormsPage : FormsPage
    {
        protected override FormBuilderRequestContext RenderingContext
        {
            get { return (FormBuilderRequestContext)FunctionContextContainer.GetParameterValue("RenderingContext", typeof(FormBuilderRequestContext)); }
        }
    }
}