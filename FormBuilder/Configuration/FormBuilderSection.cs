using System.Configuration;

using CompositeC1Contrib.FormBuilder.Configuration;

namespace CompositeC1Contrib.FormBuilder.Configuration
{
    public class FormBuilderSection : ConfigurationSection
    {
        private const string ConfigPath = "compositeC1Contrib/formBuilder";

        [ConfigurationProperty("defaultFunctionExecutor", DefaultValue = "FormBuilder.StandardFormExecutor")]
        public string DefaultFunctionExecutor
        {
            get { return (string)this["defaultFunctionExecutor"]; }
            set { this["defaultFunctionExecutor"] = value; }
        }

        public void Save()
        {
            ConfigurationSectionHelper.Save<FormBuilderSection>(this, ConfigPath);
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        public static FormBuilderSection GetSection()
        {
            return ConfigurationSectionHelper.GetOrCreateSection<FormBuilderSection>(ConfigPath);
        }
    }
}
