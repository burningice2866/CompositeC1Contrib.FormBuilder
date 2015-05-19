using System;
using System.Collections.Generic;

using Composite.C1Console.Security;

using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens
{
    [SecurityAncestorProvider(typeof(FieldValidatorsAncestorProvider))]
    public class FieldValidatorsEntityToken : EntityToken
    {
        private readonly string _type = String.Empty;
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
            get { return Source; }
        }

        public string FieldName
        {
            get { return Id; }
        }

        public FieldValidatorsEntityToken(string formName, string fieldName) : this(formName, fieldName, null) { }

        public FieldValidatorsEntityToken(string formName, string fieldName, Type validatorType)
        {
            _source = formName;
            _id = fieldName;

            if (validatorType != null)
            {
                _type = validatorType.AssemblyQualifiedName;
            }
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

            Type validatorType = null;
            if (!String.IsNullOrEmpty(type))
            {
                validatorType = System.Type.GetType(type);
            }

            return new FieldValidatorsEntityToken(source, id, validatorType);
        }
    }

    public class FieldValidatorsAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var token = entityToken as FieldValidatorsEntityToken;
            if (token != null)
            {
                yield return new FormFieldEntityToken(token.Source, token.FieldName);
            }
        }
    }
}
