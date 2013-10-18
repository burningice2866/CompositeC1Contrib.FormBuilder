using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Security;
using Composite.Core.Xml;
using Composite.Functions;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class DynamicFormFunction : IFunction
    {
        private FormModel _model;

        public string Namespace { get; private set; }
        public string Name { get; private set; }

        public EntityToken EntityToken
        {
            get { return new DynamicFormFunctionEntityToken(typeof(DynamicFormFunctionProvider).Name, _model.Name); }
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

        public DynamicFormFunction(FormModel form)
        {
            _model = form;

            var parts = form.Name.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            Namespace = String.Join(".", parts.Take(parts.Length - 1));
            Name = parts.Skip(parts.Length - 1).Take(1).Single();
        }

        public object Execute(ParameterList parameters, FunctionContextContainer context)
        {
            var dynamicFormExecutor = FunctionFacade.GetFunction("FormBuilder.DynamicFormExecutor");
            var functionParameters = new Dictionary<string, object>
            {
                { "FormName", _model.Name },
                { "IntroText", parameters.GetParameter("IntroText") },
                { "SuccessResponse", parameters.GetParameter("SuccessResponse") },
            };

            return FunctionFacade.Execute<XhtmlDocument>(dynamicFormExecutor, functionParameters);
        }
    }
}
