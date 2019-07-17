using System;

using Composite.C1Console.Security;
using Composite.C1Console.Security.SecurityAncestorProviders;

namespace CompositeC1Contrib.FormBuilder.C1Console.EntityTokens
{
    [SecurityAncestorProvider(typeof(NoAncestorSecurityAncestorProvider))]
    public class FormElementProviderEntityToken : EntityToken
    {
        public override string Id => "FormElementProviderEntityToken";

        public override string Source => String.Empty;

        public override string Type => String.Empty;

        public override string Serialize()
        {
            return String.Empty;
        }

        public static EntityToken Deserialize(string serializedData)
        {
            return new FormElementProviderEntityToken();
        }
    }
}
