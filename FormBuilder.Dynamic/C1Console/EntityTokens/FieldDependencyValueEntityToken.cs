using System;
using System.Collections.Generic;
using System.Text;

using Composite.C1Console.Security;
using Composite.Core.Serialization;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens
{
    [SecurityAncestorProvider(typeof(FieldDependencyValueAncestorProvider))]
    public class FieldDependencyValueEntityToken : EntityToken
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

        private readonly string _value;
        public string Value
        {
            get { return _value; }
        }

        public FieldDependencyValueEntityToken(string formName, string fieldName, string fromFieldName, string value = null)
        {
            _type = formName;
            _source = fieldName;
            _id = fromFieldName;
            _value = value ?? String.Empty;
        }

        public override string Serialize()
        {
            var stringBuilder = new StringBuilder();

            StringConversionServices.SerializeKeyValuePair(stringBuilder, "_EntityToken_Type_", Type);
            StringConversionServices.SerializeKeyValuePair(stringBuilder, "_EntityToken_Source_", Source);
            StringConversionServices.SerializeKeyValuePair(stringBuilder, "_EntityToken_Id_", Id);
            StringConversionServices.SerializeKeyValuePair(stringBuilder, "_EntityToken_Value_", Value);

            return stringBuilder.ToString();
        }

        public static EntityToken Deserialize(string serializedEntityToken)
        {
            var dic = StringConversionServices.ParseKeyValueCollection(serializedEntityToken);

            var type = StringConversionServices.DeserializeValueString(dic["_EntityToken_Type_"]);
            var source = StringConversionServices.DeserializeValueString(dic["_EntityToken_Source_"]);
            var id = StringConversionServices.DeserializeValueString(dic["_EntityToken_Id_"]);
            var value = StringConversionServices.DeserializeValueString(dic["_EntityToken_Value_"]);

            return new FieldDependencyValueEntityToken(type, source, id, value);
        }

        public override int GetHashCode()
        {
            if (HashCode == 0)
            {
                HashCode = base.GetHashCode() ^ Value.GetHashCode() ^ GetType().GetHashCode();
            }

            return HashCode;
        }

        public override bool Equals(object obj)
        {
            var token = obj as FieldDependencyValueEntityToken;

            return token != null && base.Equals(obj) && Value == token.Value;
        }
    }

    public class FieldDependencyValueAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var token = entityToken as FieldDependencyValueEntityToken;
            if (token != null)
            {
                yield return new FieldDependencyEntityToken(token.FormName, token.FieldName, token.FromFieldName);
            }
        }
    }
}
