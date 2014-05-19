using System;
using System.Collections.Generic;

using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.C1Console.EntityTokens
{
    [SecurityAncestorProvider(typeof(FormInstanceAncestorProvider))]
    public class FormInstanceEntityToken : EntityToken
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

        public string FormName
        {
            get { return _id; }
        }

        public FormInstanceEntityToken(string source, string formName)
        {
            _source = source;
            _id = formName;
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

            return new FormInstanceEntityToken(source, id);
        }
    }

    public class FormInstanceAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var formInstanceToken = entityToken as FormInstanceEntityToken;
            if (formInstanceToken != null)
            {
                yield return new FormElementProviderEntityToken();
            }
        }
    }
}
