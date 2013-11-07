using System;
using System.Linq;
using System.Workflow.Activities;
using System.Xml.Linq;

using Composite.C1Console.Workflow;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public class EditFormFieldWorkflow : Basic1StepEditPageWorkflow
    {
        public override string FormDefinitionFileName
        {
            get { return "\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\EditFormFieldWorkflow.xml"; }
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (!BindingExist("FieldName"))
            {
                var fieldToken = (FormFieldEntityToken)EntityToken;

                var definition = DynamicFormsFacade.GetFormByName(fieldToken.FormName);
                var field = definition.Model.Fields.Single(f => f.Name == fieldToken.FieldName);
                var defaultValue = String.Empty;

                XElement el;
                if (definition.DefaultValues.TryGetValue(field.Name, out el))
                {
                    defaultValue = el.ToString();
                }

                Bindings.Add("FieldName", field.Name);
                Bindings.Add("Label", field.Label == null ? String.Empty : field.Label.Label);
                Bindings.Add("PlaceholderText", field.PlaceholderText);
                Bindings.Add("Help", field.Help);
                Bindings.Add("DefaultValue", defaultValue);
                Bindings.Add("InputElementType", field.InputTypeHandler.GetType().AssemblyQualifiedName);
            }
        }

        public override void OnSave(object sender, EventArgs e)
        {
            var fieldToken = (FormFieldEntityToken)EntityToken;

            var fieldName = GetBinding<string>("FieldName");
            var label = GetBinding<string>("Label");
            var placeholderText = GetBinding<string>("PlaceholderText");
            var help = GetBinding<string>("Help");
            var defaultValue = GetBinding<string>("DefaultValue");

            var inputElementType = Type.GetType(GetBinding<string>("InputElementType"));

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

            definition.DefaultValues.Remove(field.Name);
            if (!String.IsNullOrEmpty(defaultValue))
            {
                definition.DefaultValues.Add(field.Name, XElement.Parse(defaultValue));
            }

            var inputTypeAttribute = field.Attributes.OfType<InputElementProviderAttribute>().FirstOrDefault();
            if (inputTypeAttribute != null)
            {
                field.Attributes.Remove(inputTypeAttribute);
            }

            inputTypeAttribute = new TypeBasedInputElementProviderAttribute(inputElementType);
            field.Attributes.Add(inputTypeAttribute);

            DynamicFormsFacade.SaveForm(definition);

            CreateSpecificTreeRefresher().PostRefreshMesseges(EntityToken);
            SetSaveStatus(true);
        }

        public override void OnValidate(object sender, ConditionalEventArgs e)
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
    }
}
