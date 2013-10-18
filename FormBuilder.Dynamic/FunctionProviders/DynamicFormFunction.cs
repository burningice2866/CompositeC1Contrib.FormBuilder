using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Security;
using Composite.Core.Xml;
using Composite.Functions;

using CompositeC1Contrib.FormBuilder.Dynamic;
using CompositeC1Contrib.FormBuilder.Dynamic.Configuration;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class DynamicFormFunction : IFunction
    {
        private DynamicFormDefinition _definition;

        public string Namespace { get; private set; }
        public string Name { get; private set; }

        public EntityToken EntityToken
        {
            get { return new DynamicFormFunctionEntityToken(typeof(DynamicFormFunctionProvider).Name, _definition.Name); }
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

        public DynamicFormFunction(DynamicFormDefinition definition)
        {
            _definition = definition;

            var parts = _definition.Name.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            Namespace = String.Join(".", parts.Take(parts.Length - 1));
            Name = parts.Skip(parts.Length - 1).Take(1).Single();
        }

        public object Execute(ParameterList parameters, FunctionContextContainer context)
        {
            var formExecutorFunction = _definition.FormExecutor;
            if (String.IsNullOrEmpty(formExecutorFunction))
            {
                formExecutorFunction = DynamicSection.GetSection().DefaultFunctionExecutor;
            }

            var dynamicFormExecutor = FunctionFacade.GetFunction(formExecutorFunction);
            var functionParameters = new Dictionary<string, object>
            {
                { "FormName", _definition.Name },
                { "IntroText", parameters.GetParameter("IntroText") },
                { "SuccessResponse", parameters.GetParameter("SuccessResponse") },
            };

            return FunctionFacade.Execute<XhtmlDocument>(dynamicFormExecutor, functionParameters);
        }
    }
}
