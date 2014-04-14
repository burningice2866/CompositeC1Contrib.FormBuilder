
using CompositeC1Contrib.FormBuilder.FunctionProviders;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public abstract class StandardFormsPage : FormsPage
    {
        public override FormBuilderRequestContext RenderingContext
        {
            get { return (FormBuilderRequestContext)FunctionContextContainer.GetParameterValue(StandardFormFunction.RenderingContextKey, typeof(FormBuilderRequestContext)); }
        }
    }
}