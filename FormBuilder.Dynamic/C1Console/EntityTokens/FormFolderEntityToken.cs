using System;
using System.Collections.Generic;

using Composite.C1Console.Security;

using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.ElementProvider;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.EntityTokens
{
    public enum FormFolderType
    {
        Fields,
        SubmitHandlers
    }

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
            return DoSerialize();
        }

        public static EntityToken Deserialize(string serializedEntityToken)
        {
            string type;
            string source;
            string id;

            DoDeserialize(serializedEntityToken, out type, out source, out id);

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
                yield return new FormInstanceEntityToken(typeof(FormBuilderElementProvider).Name, fieldFolderToken.Id);
            }
        }
    }
}
