using System.Collections.Generic;

using Composite.C1Console.Security;

using CompositeC1Contrib.FormBuilder.Attributes;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens
{
    [SecurityAncestorProvider(typeof(StringBasedDataSourceEntryAncestorProvider))]
    public class StringBasedDataSourceEntryEntityToken : EntityToken
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
            get { return _type; }
        }

        public string FieldName
        {
            get { return _source; }
        }

        public StringBasedDataSourceEntryEntityToken(string formName, string fieldName, string key)
        {
            _type = formName;
            _source = fieldName;
            _id = key;
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

            return new StringBasedDataSourceEntryEntityToken(type, source, id);
        }
    }

    public class StringBasedDataSourceEntryAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var dsEntryToken = entityToken as StringBasedDataSourceEntryEntityToken;
            if (dsEntryToken != null)
            {
                yield return new DataSourceEntityToken(typeof(StringBasedDataSourceAttribute), dsEntryToken.FormName, dsEntryToken.FieldName);
            }
        }
    }
}
