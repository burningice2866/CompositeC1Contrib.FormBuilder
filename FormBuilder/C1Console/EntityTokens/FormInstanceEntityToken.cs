using System;
using System.Collections.Generic;
using System.Linq;

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

        public string Name
        {
            get { return _id; }
        }

        public string Namespace
        {
            get
            {
                var parts = Name.Split(new[] { '.' });
                var ns = String.Join(".", parts.Take(parts.Length - 1));

                return ns;
            }
        }

        public FormInstanceEntityToken(string source, string fullName)
        {
            _source = source;
            _id = fullName;
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
            if (formInstanceToken == null)
            {
                yield break;
            }

            yield return new NamespaceFolderEntityToken(formInstanceToken.Source, formInstanceToken.Namespace);
        }
    }
}
