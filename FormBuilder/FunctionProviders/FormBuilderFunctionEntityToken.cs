using System;

using Composite.C1Console.Security;
using Composite.Functions;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    [SecurityAncestorProvider(typeof(StandardFunctionSecurityAncestorProvider))]
    public class FormBuilderFunctionEntityToken : EntityToken
    {
        public override string Id { get; }

        public override string Source { get; }

        public override string Type => String.Empty;

        public FormBuilderFunctionEntityToken(string source, string id)
        {
            Source = source;
            Id = id;
        }

        public override string Serialize()
        {
            return DoSerialize();
        }

        public static EntityToken Deserialize(string serializedEntityToken)
        {
            DoDeserialize(serializedEntityToken, out _, out var source, out var id);

            return new FormBuilderFunctionEntityToken(source, id);
        }
    }
}
