﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Composite.Core.Xml;

using CompositeC1Contrib.FormBuilder.Configuration;
using CompositeC1Contrib.FormBuilder.Dynamic.Configuration;
using CompositeC1Contrib.FormBuilder.Dynamic.SubmitHandlers;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public class DynamicFormDefinition : IDynamicDefinition
    {
        public FormModel Model { get; }

        public string Name { get; set; }
        
        public IDictionary<string, XElement> DefaultValues { get; }
        public IList<FormSubmitHandler> SubmitHandlers { get; }

        public XhtmlDocument IntroText { get; set; }
        public XhtmlDocument SuccessResponse { get; set; }

        public IFormSettings Settings { get; set; }

        public DynamicFormDefinition(string name) : this(new FormModel(name)) { }

        public DynamicFormDefinition(FormModel model)
        {
            Model = model;

            Name = model.Name;

            DefaultValues = new Dictionary<string, XElement>();
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
