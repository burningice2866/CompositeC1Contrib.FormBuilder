using System;
using System.Collections.Generic;

using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.Wizard.C1Console.EntityTokens
{
    [SecurityAncestorProvider(typeof(FormWizardAncestorProvider))]
    public class FormWizardEntityToken : EntityToken
    {
        public override string Type
        {
            get { return String.Empty; }
        }

        public override string Source
        {
            get { return String.Empty; }
        }

        private readonly string _id;
        public override string Id
        {
            get { return _id; }
        }

        public string WizardName
        {
            get { return _id; }
        }

        public FormWizardEntityToken(string wizardName)
        {
            _id = wizardName;
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

            return new FormWizardEntityToken(id);
        }
    }

    public class FormWizardAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var formInstanceToken = entityToken as FormWizardEntityToken;
            if (formInstanceToken != null)
            {
                yield return new FormWizardsElementProviderEntityToken();
            }
        }
    }
}
