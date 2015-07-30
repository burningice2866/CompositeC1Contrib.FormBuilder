using System;
using System.Collections.Generic;

using Composite.C1Console.Security;
using Composite.Data;

using CompositeC1Contrib.FormBuilder.Data.Types;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens
{
    [SecurityAncestorProvider(typeof(FormWizardStepAncestorProvider))]
    public class FormWizardStepEntityToken : EntityToken
    {
        public override string Type
        {
            get { return String.Empty; }
        }

        private readonly string _source;
        public override string Source
        {
            get { return _source; }
        }

        private readonly string _id;
        public override string Id
        {
            get { return _id; }
        }

        public string WizardName
        {
            get { return Source; }
        }

        public string StepName
        {
            get { return Id; }
        }

        public FormWizardStepEntityToken(string wizardName, string stepName)
        {
            _source = wizardName;
            _id = stepName;
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

            return new FormWizardStepEntityToken(source, id);
        }
    }

    public class FormWizardStepAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var fieldToken = entityToken as FormWizardStepEntityToken;
            if (fieldToken != null)
            {
                var data = ModelReferenceFacade.GetModelReference(fieldToken.WizardName);
                var token = data.GetDataEntityToken();

                yield return token;
            }
        }
    }
}
