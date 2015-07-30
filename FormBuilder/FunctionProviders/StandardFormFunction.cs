using Composite.Core.Xml;

using CompositeC1Contrib.FormBuilder.Configuration;
using CompositeC1Contrib.FormBuilder.Web;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class StandardFormFunction<T> : BaseFormFunction<T, Form> where T : FormRequestContext
    {
        protected override string StandardFormExecutor
        {
            get { return FormBuilderConfiguration.GetSection().DefaultFunctionExecutor; }
        }

        public StandardFormFunction(string name) : this(name, null, null) { }
        public StandardFormFunction(string name, XhtmlDocument introText, XhtmlDocument successResponse) : base(name, introText, successResponse) { }
    }
}
