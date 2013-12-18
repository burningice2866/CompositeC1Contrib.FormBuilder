using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using Composite.C1Console.Security;
using Composite.Core.Xml;
using Composite.Functions;

using CompositeC1Contrib.FormBuilder.Configuration;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class StandardFormFunction<T> : IFunction where T : FormBuilderRequestContext
    {
        private static readonly Type XElementParameterRuntimeTreeNode = Type.GetType("Composite.Functions.XElementParameterRuntimeTreeNode, Composite");

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
                var list = new List<ParameterProfile>();

                list.Add(new ParameterProfile("IntroText", typeof(XhtmlDocument), false, new ConstantValueProvider(String.Empty), StandardWidgetFunctions.GetDefaultWidgetFunctionProviderByType(typeof(XhtmlDocument)), "Intro text", new HelpDefinition("Intro text")));
                list.Add(new ParameterProfile("SuccessResponse", typeof(XhtmlDocument), false, new ConstantValueProvider(String.Empty), StandardWidgetFunctions.GetDefaultWidgetFunctionProviderByType(typeof(XhtmlDocument)), "Success response", new HelpDefinition("Success response")));

                return list;
            }
        }

        public StandardFormFunction(string name)
        {
            var parts = name.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            Namespace = String.Join(".", parts.Take(parts.Length - 1));
            Name = parts.Skip(parts.Length - 1).Take(1).Single();
        }

        public virtual object Execute(ParameterList parameters, FunctionContextContainer context)
        {
            var renderingContext = (FormBuilderRequestContext)Activator.CreateInstance(typeof(T), new[] { Namespace + "." + Name });
            renderingContext.Execute();

            var newContext = new FunctionContextContainer(context, new Dictionary<string, object>
            {
                { "RenderingContext", renderingContext },
                { "FormModel", renderingContext.RenderingModel }
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

                if (type == XElementParameterRuntimeTreeNode)
                {
                    value = (XElement)type.GetField("_element", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(value);
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
