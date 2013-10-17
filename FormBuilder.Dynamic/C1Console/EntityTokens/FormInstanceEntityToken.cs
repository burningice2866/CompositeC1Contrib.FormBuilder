using System;

using Composite.C1Console.Security;
using System.Collections.Generic;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens
{
    [SecurityAncestorProvider(typeof(FormInstanceAncestorProvider))]
    public class FormInstanceEntityToken : EntityToken
    {
        public override string Type
        {
            get { return String.Empty; }
        }

        public override string Source
        {
            get { return typeof(FormBuilderElementProvider).Name; }
        }

        private string _id;
        public override string Id
        {
            get { return _id; }
        }

        public string FormName
        {
            get { return _id; }
        }

        public FormInstanceEntityToken(string formName)
        {
            _id = formName;
        }

        public override string Serialize()
        {
            return base.DoSerialize();
        }

        public static EntityToken Deserialize(string serializedEntityToken)
        {
            string type;
            string source;
            string id;

            EntityToken.DoDeserialize(serializedEntityToken, out type, out source, out id);

            return new FormInstanceEntityToken(id);
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
