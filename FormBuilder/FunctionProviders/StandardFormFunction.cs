using System;
using System.Collections.Generic;
using System.Web;

using Composite.Core.Xml;
using Composite.Functions;

using CompositeC1Contrib.FormBuilder.Configuration;
using CompositeC1Contrib.FormBuilder.Web;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public abstract class StandardFormFunction : BaseFormFunction
    {
        protected static readonly string FormExecutorFunction = FormBuilderConfiguration.GetSection().DefaultFunctionExecutor;

        protected StandardFormFunction(string name) : this(name, null, null) { }
        protected StandardFormFunction(string name, XhtmlDocument introText, XhtmlDocument successResponse) : base(name, introText, successResponse) { }
    }

    public class StandardFormFunction<T> : StandardFormFunction where T : FormBuilderRequestContext
    {
        public StandardFormFunction(string name) : this(name, null, null) { }
        public StandardFormFunction(string name, XhtmlDocument introText, XhtmlDocument successResponse) : base(name, introText, successResponse) { }

        public override object Execute(ParameterList parameters, FunctionContextContainer context)
        {
            var renderingContext = (FormBuilderRequestContext)Activator.CreateInstance(typeof(T), new object[] { Namespace + "." + Name });
            var httpContext = new HttpContextWrapper(HttpContext.Current);

            renderingContext.Execute(httpContext);

            var newContext = new FunctionContextContainer(context, new Dictionary<string, object>
            {
                { RenderingContextKey, renderingContext },
                { FormModelKey, renderingContext.RenderingModel }
            });

            var formExecutorFunction = FormExecutorFunction;

            var formExecutor = FunctionFacade.GetFunction(formExecutorFunction);
            var functionParameters = new Dictionary<string, object>
            {
                { "FormName", Namespace +"."+ Name }
            };

            CopyFunctionParameters(parameters, newContext, functionParameters);

            return FunctionFacade.Execute<XhtmlDocument>(formExecutor, functionParameters, newContext);
        }
    }
}
