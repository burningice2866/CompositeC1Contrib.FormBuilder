using CompositeC1Contrib.FormBuilder.Configuration;
using CompositeC1Contrib.FormBuilder.Web;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class StandardFormFunction<T> : BaseFormFunction<T, Form> where T : FormRequestContext
    {
        protected override string StandardFormExecutor => FormBuilderConfiguration.GetSection().DefaultFunctionExecutor;

        public StandardFormFunction(IModel form) : base(form) { }
    }
}
