using System;
using System.Collections.Generic;

using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.Dynamic.Wizard.C1Console.EntityTokens
{
    public enum FormWizardFolderType
    {
        Steps,
        SubmitHandlers
    }

    [SecurityAncestorProvider(typeof(FormFolderAncestorProvider))]
    public class FormWizardFolderEntityToken : EntityToken
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

        public string WizardName
        {
            get { return Id; }
        }

        public FormWizardFolderType FolderType
        {
            get { return (FormWizardFolderType)Enum.Parse(typeof(FormWizardFolderType), Source); }
        }

        public FormWizardFolderEntityToken(string wizardName, FormWizardFolderType folderType)
        {
            _id = wizardName;
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

            return new FormWizardFolderEntityToken(id, (FormWizardFolderType)Enum.Parse(typeof(FormWizardFolderType), source));
        }
    }

    public class FormFolderAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var fieldFolderToken = entityToken as FormWizardFolderEntityToken;
            if (fieldFolderToken != null)
            {
                yield return new FormWizardEntityToken(fieldFolderToken.WizardName);
            }
        }
    }
}
