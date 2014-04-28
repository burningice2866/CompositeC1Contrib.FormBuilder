
using CompositeC1Contrib.FormBuilder.FunctionProviders;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public abstract class StandardFormsPage : FormsPage
    {
        protected override FormBuilderRequestContext RenderingContext
        {
            get { return (FormBuilderRequestContext)FunctionContextContainer.GetParameterValue(BaseFormFunction.RenderingContextKey, typeof(FormBuilderRequestContext)); }
        }
    }
}