using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Security;
using Composite.Core.Xml;
using Composite.Functions;

using CompositeC1Contrib.FormBuilder.Configuration;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class FormWizardFunction : IFunction
    {
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

                return list;
            }
        }

        public FormWizardFunction(string wizardName)
        {
            var parts = wizardName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            Namespace = String.Join(".", parts.Take(parts.Length - 1));
            Name = parts.Skip(parts.Length - 1).Take(1).Single();
        }

        public virtual object Execute(ParameterList parameters, FunctionContextContainer context)
        {
            var formExecutor = FunctionFacade.GetFunction("FormBuilder.StandardFormWizardExecutor");
            var functionParameters = new Dictionary<string, object>
            {
                { "WizardName", Namespace +"."+ Name }
            };

            return FunctionFacade.Execute<XhtmlDocument>(formExecutor, functionParameters, context);
        }
    }
}
