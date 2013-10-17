using System;
using System.Collections.Generic;

using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens
{
    [SecurityAncestorProvider(typeof(FormFieldAncestorProvider))]
    public class FormFieldEntityToken : EntityToken
    {
        public override string Type
        {
            get { return String.Empty; }
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

        public FormFieldEntityToken(string formName, string fieldName)
        {
            _source = formName;
            _id = fieldName;
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

            return new FormFieldEntityToken(source, id);
        }
    }

    public class FormFieldAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var fieldToken = entityToken as FormFieldEntityToken;
            if (fieldToken != null)
            {
                yield return new FormFolderEntityToken(fieldToken.Source, FormFolderType.Fields);
            }
        }
    }
}
