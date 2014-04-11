using System;

using Composite.C1Console.Security;
using Composite.Functions;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    [SecurityAncestorProvider(typeof(StandardFunctionSecurityAncestorProvider))]
    public class FormWizardFunctionEntityToken : EntityToken
    {
        private readonly string _id;
        public override string Id
        {
            get { return _id; ; }
        }

        private readonly string _source;
        public override string Source
        {
            get { return _source; }
        }

        public override string Type
        {
            get { return String.Empty; }
        }

        public FormWizardFunctionEntityToken(string source, string id)
        {
            _source = source;
            _id = id;
        }

        public override string Serialize()
        {
            return DoSerialize();
        }

        public static EntityToken Deserialize(string serializedEntityToken)
        {
            string type;
            string source;
            string id;

            DoDeserialize(serializedEntityToken, out type, out source, out id);

            return new FormWizardFunctionEntityToken(source, id);
        }
    }
}
