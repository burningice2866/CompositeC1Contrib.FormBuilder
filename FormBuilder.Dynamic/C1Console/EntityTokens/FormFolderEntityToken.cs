using System;
using System.Collections.Generic;

using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens
{
    public enum FormFolderType
    {
        Fields
    }

    [SecurityAncestorProvider(typeof(FormFolderAncestorProvider))]
    public class FormFolderEntityToken : EntityToken
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

        public string FormName
        {
            get { return Id; }
        }

        public FormFolderType FolderType
        {
            get { return (FormFolderType)Enum.Parse(typeof(FormFolderType), Source); }
        }

        public FormFolderEntityToken(string formName, FormFolderType folderType)
        {
            _id = formName;
            _source = folderType.ToString();
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

            return new FormFolderEntityToken(id, (FormFolderType)Enum.Parse(typeof(FormFolderType), source));
        }
    }

    public class FormFolderAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var fieldFolderToken = entityToken as FormFolderEntityToken;
            if (fieldFolderToken != null)
            {
                yield return new FormInstanceEntityToken(fieldFolderToken.Id);
            }
        }
    }
}
