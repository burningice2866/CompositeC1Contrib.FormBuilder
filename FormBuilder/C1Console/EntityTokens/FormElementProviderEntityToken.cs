using System;

using Composite.C1Console.Security;
using Composite.C1Console.Security.SecurityAncestorProviders;

namespace CompositeC1Contrib.FormBuilder.C1Console.Tokens
{
    [SecurityAncestorProvider(typeof(NoAncestorSecurityAncestorProvider))]
    public class FormElementProviderEntityToken : EntityToken
    {
        public override string Id
        {
            get { return "FormElementProviderEntityToken"; }
        }

        public override string Source
        {
            get { return String.Empty; }
        }

        public override string Type
        {
            get { return String.Empty; }
        }

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
