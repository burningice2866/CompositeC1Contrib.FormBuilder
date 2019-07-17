using System;
using System.Collections.Generic;

using Composite.Core.Xml;

using CompositeC1Contrib.FormBuilder.Configuration;
using CompositeC1Contrib.FormBuilder.Dynamic.Configuration;
using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public class DynamicWizardDefinition : IDynamicDefinition
    {
        public WizardModel Model { get; }

        public string Name { get; set; }

        public IList<FormSubmitHandler> SubmitHandlers { get; }

        public XhtmlDocument IntroText { get; set; }
        public XhtmlDocument SuccessResponse { get; set; }

        public IFormSettings Settings { get; set; }

        public DynamicWizardDefinition(string name) : this(new WizardModel(name)) { }

        public DynamicWizardDefinition(WizardModel model)
        {
            Model = model;

            Name = model.Name;

            SubmitHandlers = new List<FormSubmitHandler>();

            IntroText = new XhtmlDocument();
            SuccessResponse = new XhtmlDocument();

            var config = FormBuilderConfiguration.GetSection();
            var plugin = (DynamicFormBuilderConfiguration)config.Plugins["dynamic"];

            var settingsType = plugin.SettingsHandler;
            if (settingsType != null)
            {
                Settings = (IFormSettings)Activator.CreateInstance(settingsType);
            }
        }
    }
}
