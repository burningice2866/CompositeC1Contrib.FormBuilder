using System;
using System.Collections.Generic;

using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.C1Console.EntityTokens
{
    [SecurityAncestorProvider(typeof(FormFieldAncestorProvider))]
    public class FormFieldEntityToken : EntityToken
    {
        public override string Type => String.Empty;

        public override string Source { get; }

        public override string Id { get; }

        public string FormName => Source;

        public string FieldName => Id;

        public FormFieldEntityToken(string formName, string fieldName)
        {
            Source = formName;
            Id = fieldName;
        }

        public override string Serialize()
        {
            return DoSerialize();
        }

        public static EntityToken Deserialize(string serializedEntityToken)
        {
            DoDeserialize(serializedEntityToken, out _, out var source, out var id);

            return new FormFieldEntityToken(source, id);
        }
    }

    public class FormFieldAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            if (entityToken is FormFieldEntityToken fieldToken)
            {
                yield return new FormFolderEntityToken(fieldToken.FormName, "Fields");
            }
        }
    }
}
