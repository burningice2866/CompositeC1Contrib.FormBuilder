using System;
using System.Collections.Generic;
using Composite.C1Console.Security;
using Composite.Data;
using CompositeC1Contrib.FormBuilder.Data.Types;

namespace CompositeC1Contrib.FormBuilder.C1Console.EntityTokens
{
    [SecurityAncestorProvider(typeof(FormFolderAncestorProvider))]
    public class FormFolderEntityToken : EntityToken
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
            get { return Id; }
        }

        public string FolderType
        {
            get { return Source; }
        }

        public FormFolderEntityToken(string formName, string folderType)
        {
            _id = formName;
            _source = folderType;
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

            return new FormFolderEntityToken(id, source);
        }
    }

    public class FormFolderAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var fieldFolderToken = entityToken as FormFolderEntityToken;
            if (fieldFolderToken != null)
            {
                var data = ModelReferenceFacade.GetModelReference(fieldFolderToken.Id);
                var token = data.GetDataEntityToken();

                yield return token;
            }
        }
    }
}
