using System.Configuration;

using CompositeC1Contrib.FormBuilder.Configuration;

namespace CompositeC1Contrib.FormBuilder.Dynamic.Configuration
{
    public class DynamicSection : ConfigurationSection
    {
        private const string ConfigPath = "compositeC1Contrib/formBuilder/dynamic";

        [ConfigurationProperty("defaultFunctionExecutor", DefaultValue = "FormBuilder.DynamicFormExecutor")]
        public string DefaultFunctionExecutor
        {
            get { return (string)this["defaultFunctionExecutor"]; }
            set { this["defaultFunctionExecutor"] = value; }
        }

        public void Save()
        {
            ConfigurationSectionHelper.Save<DynamicSection>(this, ConfigPath);
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        public static DynamicSection GetSection()
        {
            return ConfigurationSectionHelper.GetOrCreateSection<DynamicSection>(ConfigPath);
        }
    }
}
