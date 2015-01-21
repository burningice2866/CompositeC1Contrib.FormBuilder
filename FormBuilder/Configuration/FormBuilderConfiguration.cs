using System;
using System.Collections.Generic;
using System.Configuration;

namespace CompositeC1Contrib.FormBuilder.Configuration
{
    public class FormBuilderConfiguration
    {
        private const string ConfigPath = "compositeC1Contrib/formBuilder";

        public string DefaultFunctionExecutor { get; set; }
        public Type RendererImplementation { get; set; }

        public CaptchaConfiguration Captcha { get; set; }

        public IDictionary<string, IPluginConfiguration> Plugins { get; private set; }

        public FormBuilderConfiguration()
        {
            Captcha = new CaptchaConfiguration();
            Plugins = new Dictionary<string, IPluginConfiguration>();
        }

        public static FormBuilderConfiguration GetSection()
        {
            return (FormBuilderConfiguration)ConfigurationManager.GetSection(ConfigPath);
        }
    }
}
