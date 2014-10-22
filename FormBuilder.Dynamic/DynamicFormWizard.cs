using System;
using System.Collections.Generic;

using CompositeC1Contrib.FormBuilder.Configuration;
using CompositeC1Contrib.FormBuilder.Dynamic.Configuration;
using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public class DynamicFormWizard : FormWizard, IDynamicFormDefinition
    {
        public IList<FormSubmitHandler> SubmitHandlers { get; private set; }
        public IFormSettings Settings { get; set; }

        public DynamicFormWizard()
        {
            SubmitHandlers = new List<FormSubmitHandler>();

            var config = FormBuilderConfiguration.GetSection();
            var plugin = (DynamicFormBuilderConfiguration)config.Plugins["dynamic"];

            var settingsType = plugin.SettingsHandler;
            if (settingsType != null)
            {
                Settings = (IFormSettings)Activator.CreateInstance(settingsType);
            }
        }

        public override void Submit()
        {
            foreach (var handler in SubmitHandlers)
            {
                handler.Submit(this);
            }

            base.Submit();
        }
    }
}
