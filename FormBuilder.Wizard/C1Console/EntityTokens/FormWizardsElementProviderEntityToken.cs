using System;

using Composite.C1Console.Security;
using Composite.C1Console.Security.SecurityAncestorProviders;

namespace CompositeC1Contrib.FormBuilder.Wizard.C1Console.EntityTokens
{
    [SecurityAncestorProvider(typeof(NoAncestorSecurityAncestorProvider))]
    public class FormWizardsElementProviderEntityToken : EntityToken
    {
        public override string Id
        {
            get { return "FormWizardsElementProviderEntityToken"; }
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
            return new FormWizardsElementProviderEntityToken();
        }
    }
}
