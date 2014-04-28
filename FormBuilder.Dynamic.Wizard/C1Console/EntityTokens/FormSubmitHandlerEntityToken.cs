using System;
using System.Collections.Generic;

using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.Dynamic.Wizard.C1Console.EntityTokens
{
    [SecurityAncestorProvider(typeof(FormSubmitHandlerAncestorProvider))]
    public class FormSubmitHandlerEntityToken : EntityToken
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
            get { return _source; }
        }

        public string Name
        {
            get { return _id; }
        }

        public FormSubmitHandlerEntityToken(Type type, string formName, string name)
        {
            _type = type.AssemblyQualifiedName;
            _source = formName;
            _id = name;
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

            DoDeserialize(serializedEntityToken, out type, out source, out id);

            return new FormSubmitHandlerEntityToken(System.Type.GetType(type), source, id);
        }
    }

    public class FormSubmitHandlerAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var dataSourceToken = entityToken as FormSubmitHandlerEntityToken;
            if (dataSourceToken != null)
            {
                yield return new FormWizardFolderEntityToken(dataSourceToken.FormName, FormWizardFolderType.SubmitHandlers);
            }
        }
    }
}
