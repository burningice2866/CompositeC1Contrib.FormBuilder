using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Composite.C1Console.Security;
using Composite.Core.Xml;
using Composite.Functions;

using CompositeC1Contrib.FormBuilder.Configuration;
using CompositeC1Contrib.FormBuilder.Web;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public abstract class StandardFormFunction
    {
        public static string RenderingContextKey = "RenderingContext";
        public static string FormModelKey = "FormModel";

        protected static readonly Type XElementParameterRuntimeTreeNode = Type.GetType("Composite.Functions.XElementParameterRuntimeTreeNode, Composite");
    }

    public class StandardFormFunction<T> : StandardFormFunction, IFunction where T : FormBuilderRequestContext
    {
        private readonly XhtmlDocument _intoText;
        private readonly XhtmlDocument _successResponse;

        public string OverrideFormExecutor { get; set; }

        public string Namespace { get; private set; }
        public string Name { get; private set; }

        public EntityToken EntityToken
        {
            get { return new FormBuilderFunctionEntityToken(typeof(FormBuilderFunctionProvider).Name, Namespace + "." + Name); }
        }

        public string Description
        {
            get { return ""; }
        }

        public Type ReturnType
        {
            get { return typeof(XhtmlDocument); }
        }

        public IEnumerable<ParameterProfile> ParameterProfiles
        {
            get
            {
                var list = new List<ParameterProfile>
                {
                    new ParameterProfile("IntroText", typeof (XhtmlDocument), false,
                        new ConstantValueProvider(_intoText ?? new XhtmlDocument()),
                        StandardWidgetFunctions.GetDefaultWidgetFunctionProviderByType(typeof (XhtmlDocument)),
                        "Intro text", new HelpDefinition("Intro text")),

                    new ParameterProfile("SuccessResponse", typeof (XhtmlDocument), false,
                        new ConstantValueProvider(_successResponse ?? new XhtmlDocument()),
                        StandardWidgetFunctions.GetDefaultWidgetFunctionProviderByType(typeof (XhtmlDocument)),
                        "Success response", new HelpDefinition("Success response"))
                };

                return list;
            }
        }

        public StandardFormFunction(string name) : this(name, null, null) { }

        public StandardFormFunction(string name, XhtmlDocument introText, XhtmlDocument successResponse)
        {
            var parts = name.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            Namespace = String.Join(".", parts.Take(parts.Length - 1));
            Name = parts.Skip(parts.Length - 1).Take(1).Single();

            _intoText = introText;
            _successResponse = successResponse;
        }

        public virtual object Execute(ParameterList parameters, FunctionContextContainer context)
        {
            var renderingContext = (FormBuilderRequestContext)Activator.CreateInstance(typeof(T), new[] { Namespace + "." + Name });
            renderingContext.Execute();

            var newContext = new FunctionContextContainer(context, new Dictionary<string, object>
            {
                { RenderingContextKey, renderingContext },
                { FormModelKey, renderingContext.RenderingModel }
            });

            typeof(ParameterList).GetField("_functionContextContainer", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(parameters, newContext);

            var formExecutorFunction = FormBuilderConfiguration.GetSection().DefaultFunctionExecutor;
            if (!String.IsNullOrEmpty(OverrideFormExecutor))
            {
                formExecutorFunction = OverrideFormExecutor;
            }

            typeof(ParameterList).GetField("_parameters", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(parameters);

            var formExecutor = FunctionFacade.GetFunction(formExecutorFunction);
            var functionParameters = new Dictionary<string, object>
            {
                { "FormName", Namespace +"."+ Name }
            };

            var internalParameters = (IDictionary)typeof(ParameterList).GetField("_parameters", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(parameters);
            foreach (DictionaryEntry item in internalParameters)
            {
                object value = item.Value.GetType().GetProperty("ValueObject").GetValue(item.Value, null);
                var type = value.GetType();

                if (type == StandardFormFunction.XElementParameterRuntimeTreeNode)
                {
                    value = type.GetField("_element", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(value);
                }

                switch ((string)item.Key)
                {
                    case "IntroText": functionParameters.Add("IntroText", value); break;
                    case "SuccessResponse": functionParameters.Add("SuccessResponse", value); break;
                }
            }

            return FunctionFacade.Execute<XhtmlDocument>(formExecutor, functionParameters, newContext);
        }
    }
}
