using System;

using CompositeC1Contrib.FormBuilder.FunctionProviders;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public abstract class StandardFormsPage : FormsPage
    {
        protected override FormRequestContext RequestContext => (FormRequestContext)FunctionContextContainer.GetParameterValue(BaseFormFunction.RequestContextKey, typeof(FormRequestContext));
    }
}