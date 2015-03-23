using System;

using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public abstract class BaseAddFormWorkflow : Basic1StepDialogWorkflow
    {
        protected BaseAddFormWorkflow(string formDefinitionFile) : base(formDefinitionFile) { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (!BindingExist("Name"))
            {
                var name = String.Empty;

                var folderToken = EntityToken as NamespaceFolderEntityToken;
                if (folderToken != null)
                {
                    name = folderToken.Namespace + ".";
                }

                Bindings.Add("Name", name);
            }
        }

        public override bool Validate()
        {
            var name = GetBinding<string>("Name");

            if (!name.Contains("."))
            {
                ShowFieldMessage("Name", "Name needs to contain one or more '.' (dot)");

                return false;
            }

            if (!FormModel.IsValidName(name))
            {
                ShowFieldMessage("Name", "Name is invalid, only a-z and 0-9 is allowed");

                return false;
            }

            var folderToken = EntityToken as NamespaceFolderEntityToken;
            if (folderToken != null)
            {
                if (!name.StartsWith(folderToken.Namespace + "."))
                {
                    ShowFieldMessage("Name", "Name needs to start with " + folderToken.Namespace + ".");

                    return false;
                }
            }

            return true;
        }
    }
}
