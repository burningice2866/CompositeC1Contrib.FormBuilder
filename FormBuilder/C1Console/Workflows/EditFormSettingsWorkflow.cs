using System;

using Composite.C1Console.Users;
using Composite.Data;

using CompositeC1Contrib.FormBuilder.Data.Types;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.C1Console.Workflows
{
    public class EditFormSettingsWorkflow : Basic1StepDocumentWorkflow
    {
        public EditFormSettingsWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder\\EditFormSettingsWorkflow.xml") { }

        private IModelReference ModelReference => (IModelReference)((DataEntityToken)EntityToken).Data;

        private string GetKey(string setting)
        {
            return "Forms." + ModelReference.Name + "." + setting;
        }

        private string GetValue(string setting)
        {
            var key = GetKey(setting);
            var value = Localization.T(key, UserSettings.ActiveLocaleCultureInfo);

            return value == key ? String.Empty : value;
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("Name"))
            {
                return;
            }

            Bindings.Add("Name", ModelReference.Name);
            Bindings.Add("IntroText", GetValue("IntroText"));
            Bindings.Add("SuccessResponse", GetValue("SuccessResponse"));
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var introText = GetBinding<string>("IntroText");
            var successResponse = GetBinding<string>("SuccessResponse");

            using (var writer = ResourceFacade.GetResourceWriter(UserSettings.ActiveLocaleCultureInfo))
            {
                writer.AddResource(GetKey("IntroText"), introText);
                writer.AddResource(GetKey("SuccessResponse"), successResponse);
            }

            var treeRefresher = CreateParentTreeRefresher();
            treeRefresher.PostRefreshMesseges(EntityToken);

            SetSaveStatus(true);
        }
    }
}
