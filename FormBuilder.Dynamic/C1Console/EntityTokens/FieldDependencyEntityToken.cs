using System;
using System.Collections.Generic;

using Composite.C1Console.Security;

using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens
{
    [SecurityAncestorProvider(typeof(FieldDependencyAncestorProvider))]
    public class FieldDependencyEntityToken : EntityToken
    {
        private readonly string _type;
        public override string Type
        {
            get { return _type; }
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
            get { return Type; }
        }

        public string FieldName
        {
            get { return Source; }
        }

        public string FromFieldName
        {
            get { return Id; }
        }

        public FieldDependencyEntityToken(string formName, string fieldName) : this(formName, fieldName, null) { }

        public FieldDependencyEntityToken(string formName, string fieldName, string fromFieldName)
        {
            _type = formName;
            _source = fieldName;
            _id = fromFieldName ?? String.Empty;
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

            return new FieldDependencyEntityToken(type, source, id);
        }
    }

    public class FieldDependencyAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var token = entityToken as FieldDependencyEntityToken;
            if (token != null)
            {
                if (String.IsNullOrEmpty(token.FromFieldName))
                {
                    yield return new FormFieldEntityToken(token.FormName, token.FieldName);
                }
                else
                {
                    yield return new FieldDependencyEntityToken(token.FormName, token.FieldName);
                }
            }
        }
    }
}
