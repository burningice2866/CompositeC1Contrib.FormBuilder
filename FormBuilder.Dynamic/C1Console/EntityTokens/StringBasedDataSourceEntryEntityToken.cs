using Composite.C1Console.Security;
using System.Collections.Generic;
using CompositeC1Contrib.FormBuilder.Attributes;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens
{
    [SecurityAncestorProvider(typeof(StringBasedDataSourceEntryAncestorProvider))]
    public class StringBasedDataSourceEntryEntityToken : EntityToken
    {
        private string _type;
        public override string Type
        {
            get { return _type; }
        }

        private string _source;
        public override string Source
        {
            get { return _source; }
        }

        private string _id;
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
            return base.DoSerialize();
        }

        public static EntityToken Deserialize(string serializedEntityToken)
        {
            string type;
            string source;
            string id;

            EntityToken.DoDeserialize(serializedEntityToken, out type, out source, out id);

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
