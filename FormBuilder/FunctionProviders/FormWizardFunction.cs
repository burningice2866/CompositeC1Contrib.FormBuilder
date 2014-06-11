using System;
using System.Collections.Generic;
using System.Web;

using Composite.Core.Xml;
using Composite.Functions;

using CompositeC1Contrib.FormBuilder.Web;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class FormWizardFunction<T> : BaseFormFunction where T : FormWizardRequestContext
    {
        public FormWizardFunction(string name) : this(name, null, null) { }
        public FormWizardFunction(string name, XhtmlDocument introText, XhtmlDocument successResponse) : base(name, introText, successResponse) { }

        public override object Execute(ParameterList parameters, FunctionContextContainer context)
        {
            var renderingContext = (FormWizardRequestContext)Activator.CreateInstance(typeof(T), new object[] { Namespace + "." + Name });
            var httpContext = new HttpContextWrapper(HttpContext.Current);

            renderingContext.Execute(httpContext);

            var newContext = new FunctionContextContainer(context, new Dictionary<string, object>
            {
                { RenderingContextKey, renderingContext },
                { FormModelKey, renderingContext.RenderingModel }
            });

            var formExecutor = FunctionFacade.GetFunction("FormBuilder.StandardFormWizardExecutor");
            var functionParameters = new Dictionary<string, object>()
            {
                { "WizardName", Namespace + "." + Name }
            };

            CopyFunctionParameters(parameters, newContext, functionParameters);

            return FunctionFacade.Execute<XhtmlDocument>(formExecutor, functionParameters, newContext);
        }
    }
}
