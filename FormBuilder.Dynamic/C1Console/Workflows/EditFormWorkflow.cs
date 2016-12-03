using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Users;
using Composite.C1Console.Workflow;
using Composite.Data;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Data.Types;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public class EditFormWorkflow : BaseEditFormWorkflow
    {
        public EditFormWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\EditFormWorkflow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("BoundToken"))
            {
                return;
            }

            Bindings.Add("BoundToken", EntityToken);
            Bindings.Add("SubmitButtonLabel", GetValue("SubmitButtonLabel"));

            var modelReference = (IModelReference)((DataEntityToken)EntityToken).Data;
            var definition = DynamicFormsFacade.GetFormByName(modelReference.Name);

            SetupFormData(definition, definition.Model);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var formToken = GetBinding<DataEntityToken>("BoundToken");
            var modelReference = (IModelReference)formToken.Data;

            var definition = DynamicFormsFacade.GetFormByName(modelReference.Name);

            var submitButtonLabel = GetBinding<string>("SubmitButtonLabel");

            SwitchAttribute<RequiresCaptchaAttribute>("RequiresCaptcha", definition.Model.Attributes);
            SwitchAttribute<ForceHttpsConnectionAttribute>("ForceHttpsConnection", definition.Model.Attributes);

            var submitButtonLabelAttr = definition.Model.Attributes.OfType<SubmitButtonLabelAttribute>().SingleOrDefault();
            if (submitButtonLabel != null)
            {
                definition.Model.Attributes.Remove(submitButtonLabelAttr);
            }

            if (!String.IsNullOrEmpty(submitButtonLabel))
            {
                submitButtonLabelAttr = new SubmitButtonLabelAttribute(submitButtonLabel);
                definition.Model.Attributes.Add(submitButtonLabelAttr);

                using (var writer = ResourceFacade.GetResourceWriter(UserSettings.ActiveLocaleCultureInfo))
                {
                    writer.AddResource(GetKey("SubmitButtonLabel"), submitButtonLabel);
                }
            }

            Save(definition);
        }

        private void SwitchAttribute<T>(string bindingName, ICollection<Attribute> attributes) where T : Attribute, new()
        {
            var flip = GetBinding<bool>(bindingName);
            var attribute = attributes.OfType<T>().SingleOrDefault();

            if (flip)
            {
                if (attribute == null)
                {
                    attribute = new T();
                    attributes.Add(attribute);
                }
            }
            else
            {
                if (attribute != null)
                {
                    attributes.Remove(attribute);
                }
            }
        }
    }
}
