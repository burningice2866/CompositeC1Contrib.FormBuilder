using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

using Composite.C1Console.Security;
using Composite.Core.Xml;
using Composite.Functions;

using CompositeC1Contrib.FormBuilder.Web;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public abstract class BaseFormFunction : IFunction
    {
        protected static readonly Type XElementParameterRuntimeTreeNode = Type.GetType("Composite.Functions.XElementParameterRuntimeTreeNode, Composite");

        private readonly XhtmlDocument _intoText;
        private readonly XhtmlDocument _successResponse;

        public const string InstanceKey = "Instance";
        public const string RequestContextKey = "RequestContext";

        public abstract object Execute(ParameterList parameters, FunctionContextContainer context);

        public string OverrideFormExecutor { get; set; }
        public string Namespace { get; }
        public string Name { get; }

        public string Description => String.Empty;

        public EntityToken EntityToken => new FormBuilderFunctionEntityToken(typeof(FormBuilderFunctionProvider).Name, Namespace + "." + Name);

        public Type ReturnType => typeof(XhtmlDocument);

        public IEnumerable<ParameterProfile> ParameterProfiles
        {
            get
            {
                var list = new List<ParameterProfile>
                {
                    new ParameterProfile("IntroText", typeof(XhtmlDocument), false,
                        new ConstantValueProvider(_intoText ?? new XhtmlDocument()),
                        StandardWidgetFunctions.GetDefaultWidgetFunctionProviderByType(typeof(XhtmlDocument)),
                        "Intro text", new HelpDefinition("Intro text")),

                    new ParameterProfile("SuccessResponse", typeof(XhtmlDocument), false,
                        new ConstantValueProvider(_successResponse ?? new XhtmlDocument()),
                        StandardWidgetFunctions.GetDefaultWidgetFunctionProviderByType(typeof(XhtmlDocument)),
                        "Success response", new HelpDefinition("Success response"))
                };

                return list;
            }
        }

        protected abstract string StandardFormExecutor { get; }

        protected BaseFormFunction(IModel form)
        {
            var functionName = form.Name;
            if (!functionName.StartsWith("Forms."))
            {
                functionName = "Forms." + functionName;
            }

            var parts = functionName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            Namespace = String.Join(".", parts.Take(parts.Length - 1));
            Name = parts.Skip(parts.Length - 1).Take(1).Single();

            if (_intoText == null)
            {
                var value = Localization.EvaluateT(form, "IntroText", null);
                if (value != null)
                {
                    try
                    {
                        _intoText = XhtmlDocument.Parse(value);
                    }
                    catch { }
                }
            }

            if (_successResponse == null)
            {
                var value = Localization.EvaluateT(form, "SuccessResponse", null);
                if (value != null)
                {
                    try
                    {
                        _successResponse = XhtmlDocument.Parse(value);
                    }
                    catch { }
                }
            }
        }
    }

    public abstract class BaseFormFunction<TRequestContext, TInstance> : BaseFormFunction
        where TRequestContext : BaseFormBuilderRequestContext<TInstance>
        where TInstance : IModelInstance
    {
        private IModel _form;

        protected BaseFormFunction(IModel form) : base(form)
        {
            _form = form;
        }

        protected static void CopyFunctionParameters(ParameterList parameters, FunctionContextContainer newContext, IDictionary<string, object> functionParameters)
        {
            typeof(ParameterList).GetField("_functionContextContainer", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(parameters, newContext);

            var internalParameters = (IDictionary)typeof(ParameterList).GetField("_parameters", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(parameters);
            foreach (DictionaryEntry item in internalParameters)
            {
                object value = item.Value.GetType().GetProperty("ValueObject").GetValue(item.Value, null);
                var type = value.GetType();

                if (type == XElementParameterRuntimeTreeNode)
                {
                    value = type.GetField("_element", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(value);
                }

                switch ((string)item.Key)
                {
                    case "IntroText": functionParameters.Add("IntroText", value); break;
                    case "SuccessResponse": functionParameters.Add("SuccessResponse", value); break;
                }
            }
        }

        public override object Execute(ParameterList parameters, FunctionContextContainer context)
        {
            var requestContext = (BaseFormBuilderRequestContext<TInstance>)Activator.CreateInstance(typeof(TRequestContext), _form);
            var httpContext = new HttpContextWrapper(HttpContext.Current);

            requestContext.Execute(httpContext);

            var newContext = new FunctionContextContainer(context, new Dictionary<string, object>
            {
                { RequestContextKey, requestContext },
                { InstanceKey, requestContext.ModelInstance }
            });

            var formExecutorFunction = StandardFormExecutor;
            if (!String.IsNullOrEmpty(OverrideFormExecutor))
            {
                formExecutorFunction = OverrideFormExecutor;
            }

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
