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
        public override string Type => String.Empty;

        public override string Source { get; }

        public override string Id { get; }

        public string FormName => Id;

        public string FolderType => Source;

        public FormFolderEntityToken(string formName, string folderType)
        {
            Id = formName;
            Source = folderType;
        }

        public override string Serialize()
        {
            return DoSerialize();
        }

        public static EntityToken Deserialize(string serializedEntityToken)
        {
            DoDeserialize(serializedEntityToken, out _, out var source, out var id);

            return new FormFolderEntityToken(id, source);
        }
    }

    public class FormFolderAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            if (entityToken is FormFolderEntityToken fieldFolderToken)
            {
                var data = ModelReferenceFacade.GetModelReference(fieldFolderToken.Id);
                var token = data.GetDataEntityToken();

                yield return token;
            }
        }
    }
}
