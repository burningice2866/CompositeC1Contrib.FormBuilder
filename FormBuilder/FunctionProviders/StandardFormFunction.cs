﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Composite.C1Console.Security;
using Composite.Core.Xml;
using Composite.Functions;

using CompositeC1Contrib.FormBuilder.Configuration;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class StandardFormFunction : IFunction
    {
        private FormModel _model;
        public Action<FormModel> OnMappedValues { get; set; }
        public Action<FormModel> SetDefaultValues { get; set; }
        public Action OnSubmit { get; set; }
        public string OverrideFormExecutor { get; set; }

        public string Namespace { get; private set; }
        public string Name { get; private set; }

        public EntityToken EntityToken
        {
            get { return new FormBuilderFunctionEntityToken(typeof(FormBuilderFunctionProvider).Name, _model.Name); }
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

        public StandardFormFunction(FormModel model)
        {
            var parts = model.Name.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            Namespace = String.Join(".", parts.Take(parts.Length - 1));
            Name = parts.Skip(parts.Length - 1).Take(1).Single();

            _model = model;
        }

        public object Execute(ParameterList parameters, FunctionContextContainer context)
        {
            var renderingContext = FormBuilderRequestContext.Setup(_model, OnMappedValues, OnSubmit, SetDefaultValues);

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

            var formExecutor = FunctionFacade.GetFunction(formExecutorFunction);
            var functionParameters = new Dictionary<string, object>
            {
                { "FormName", _model.Name },
                { "IntroText", parameters.GetParameter("IntroText") },
                { "SuccessResponse", parameters.GetParameter("SuccessResponse") },
            };

            return FunctionFacade.Execute<XhtmlDocument>(formExecutor, functionParameters, newContext);
        }
    }
}
