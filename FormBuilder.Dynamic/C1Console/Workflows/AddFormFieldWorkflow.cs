using System;
using System.Collections.Generic;
using System.Linq;
using System.Workflow.Activities;

using Composite.C1Console.Workflow;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;
using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public sealed partial class AddFormFieldWorkflow : Composite.C1Console.Workflow.Activities.FormsWorkflow
    {
        private static Dictionary<string, string> _inputElementTypes = new Dictionary<string, string>();

        static AddFormFieldWorkflow()
        {
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in asms)
            {
                try
                {
                    var types = asm.GetTypes()
                        .Where(t => typeof(IInputElementHandler).IsAssignableFrom(t) && !t.IsInterface);

                    foreach (var t in types)
                    {
                        _inputElementTypes.Add(t.AssemblyQualifiedName, t.Name);
                    }
                }
                catch { }
            }
        }

        public AddFormFieldWorkflow()
        {
            InitializeComponent();
        }

        public static Dictionary<string, string> GetInputElementTypes()
        {
            return _inputElementTypes;
        }

        private void validateSave(object sender, ConditionalEventArgs e)
        {
            var folderToken = (FormFolderEntityToken)EntityToken;

            var fieldName = GetBinding<string>("FieldName");
            var definition = DynamicFormsFacade.GetFormByName(folderToken.FormName);
            var field = definition.Model.Fields.SingleOrDefault(f => f.Name == fieldName);

            if (field != null)
            {
                ShowFieldMessage("Field name", "Field name already exists");

                e.Result = false;

                return;
            }


            e.Result = true;
        }

        private void initCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            if (!BindingExist("FieldName"))
            {
                Bindings.Add("FieldName", String.Empty);
                Bindings.Add("InputElementType", typeof(TextboxInputElement).AssemblyQualifiedName);
            }
        }

        private void saveCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            var folderToken = (FormFolderEntityToken)EntityToken;

            var fieldName = GetBinding<string>("FieldName");
            var definition = DynamicFormsFacade.GetFormByName(folderToken.FormName);
            var field = new FormField(definition.Model, fieldName, typeof(string), new List<Attribute>());

            var elementType = Type.GetType(GetBinding<string>("InputElementType"));
            var inputTypeAttribute = new TypeBasedInputElementProviderAttribute(elementType);

            field.Attributes.Add(inputTypeAttribute);
            definition.Model.Fields.Add(field);

            DynamicFormsFacade.SaveForm(definition);

            var fieldToken = new FormFieldEntityToken(folderToken.FormName, fieldName);
            var workflowToken = new WorkflowActionToken(typeof(EditFormFieldWorkflow));

            CreateAddNewTreeRefresher(EntityToken).PostRefreshMesseges(fieldToken);
            ExecuteAction(fieldToken, workflowToken);
        }
    }
}
