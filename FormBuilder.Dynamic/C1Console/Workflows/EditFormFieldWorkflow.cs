using System;
using System.Linq;
using System.Workflow.Activities;

using Composite.C1Console.Workflow;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public sealed partial class EditFormFieldWorkflow : Composite.C1Console.Workflow.Activities.FormsWorkflow
    {
        public EditFormFieldWorkflow()
        {
            InitializeComponent();
        }

        private void initCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            if (!BindingExist("FieldName"))
            {
                var fieldToken = (FormFieldEntityToken)EntityToken;

                var definition = DynamicFormsFacade.GetFormByName(fieldToken.FormName);
                var field = definition.Model.Fields.Single(f => f.Name == fieldToken.FieldName);

                Bindings.Add("FieldName", field.Name);
                Bindings.Add("Label", field.Label == null ? String.Empty : field.Label.Label);
                Bindings.Add("PlaceholderText", field.PlaceholderText);
                Bindings.Add("Help", field.Help);
            }
        }

        private void validateSave(object sender, ConditionalEventArgs e)
        {
            var fieldToken = (FormFieldEntityToken)EntityToken;
            var fieldName = GetBinding<string>("FieldName");

            if (fieldName != fieldToken.FieldName)
            {
                var definition = DynamicFormsFacade.GetFormByName(fieldToken.FormName);
                var field = definition.Model.Fields.SingleOrDefault(f => f.Name == fieldName);

                if (field != null)
                {
                    ShowFieldMessage("Field name", "Field name already exists");

                    e.Result = false;

                    return;
                }

            }

            e.Result = true;
        }

        private void saveCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            var fieldToken = (FormFieldEntityToken)EntityToken;

            var fieldName = GetBinding<string>("FieldName");
            var label = GetBinding<string>("Label");
            var placeholderText = GetBinding<string>("PlaceholderText");
            var help = GetBinding<string>("Help");

            var definition = DynamicFormsFacade.GetFormByName(fieldToken.FormName);
            var field = definition.Model.Fields.Single(f => f.Name == fieldToken.FieldName);

            field.Name = fieldName;

            var labelAttr = field.Attributes.OfType<FieldLabelAttribute>().SingleOrDefault();
            if (labelAttr != null)
            {
                field.Attributes.Remove(labelAttr);
            }

            if (!String.IsNullOrEmpty(label))
            {
                labelAttr = new FieldLabelAttribute(label);
                field.Attributes.Add(labelAttr);
            }

            var placeholderAttr = field.Attributes.OfType<PlaceholderTextAttribute>().SingleOrDefault();
            if (placeholderAttr != null)
            {
                field.Attributes.Remove(placeholderAttr);
            }

            if (!String.IsNullOrEmpty(placeholderText))
            {
                placeholderAttr = new PlaceholderTextAttribute(placeholderText);
                field.Attributes.Add(placeholderAttr);
            }

            var helpAttribute = field.Attributes.OfType<FieldHelpAttribute>().FirstOrDefault();
            if (helpAttribute != null)
            {
                field.Attributes.Remove(helpAttribute);
            }            

            if (!String.IsNullOrEmpty(help))
            {
                helpAttribute = new FieldHelpAttribute(help);
                field.Attributes.Add(helpAttribute);
            }

            DynamicFormsFacade.SaveForm(definition);

            CreateSpecificTreeRefresher().PostRefreshMesseges(EntityToken);
            SetSaveStatus(true);
        }
    }
}
