using System;
using System.Collections.Generic;

using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens
{
    [SecurityAncestorProvider(typeof(DataSourceAncestorProvider))]
    public class DataSourceEntityToken : EntityToken
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
            get { return _source; }
        }

        public string FieldName
        {
            get { return _id; }
        }

        public DataSourceEntityToken(Type type, string formName, string fieldName)
        {
            _type = type.AssemblyQualifiedName;
            _source = formName;
            _id = fieldName;
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

            return new DataSourceEntityToken(System.Type.GetType(type), source, id);
        }
    }

    public class DataSourceAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var token = entityToken as DataSourceEntityToken;
            if (token != null)
            {
                yield return new FormFieldEntityToken(token.FormName, token.FieldName);
            }
        }
    }
}
