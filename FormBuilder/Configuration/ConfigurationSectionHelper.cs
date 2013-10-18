using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web.Configuration;
using System.Xml.Linq;
using System.Xml.XPath;

using Composite.Core;
using Composite.Core.IO;

namespace CompositeC1Contrib.FormBuilder.Configuration
{
    public static class ConfigurationSectionHelper
    {
        /// <summary>
        /// Save data into the configuration file
        /// </summary>
        /// <typeparam name="T">Descendants of ConfigurationSection</typeparam>
        /// <param name="configurationSection">ConfigurationSection for save data</param>
        /// <param name="configPath">config path ex: 'shop/shopProperties'</param>
        public static void Save<T>(ConfigurationSection configurationSection, string configPath) where T : ConfigurationSection
        {
            System.Configuration.Configuration config = null;

            try
            {
                config = WebConfigurationManager.OpenWebConfiguration("~");
            }
            catch (ConfigurationErrorsException e)
            {
                Log.LogCritical("Save section", e);

                throw;
            }

            var section = config.GetSection(configPath) as T;
            if (section == null)
            {
                throw new NullReferenceException("Provided configpath was not found");
            }

            var type = configurationSection.GetType();
            try
            {
                var propertiesSection = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
                foreach (var propertyInfo in propertiesSection)
                {
                    var value = propertyInfo.GetValue(configurationSection, null);

                    propertyInfo.SetValue(section, value, null);
                }
            }
            catch (Exception e)
            {
                Log.LogCritical("Save section", e);

                throw;
            }

            section.SectionInformation.RestartOnExternalChanges = false;

            config.Save(ConfigurationSaveMode.Modified);
        }

        /// <summary>
        /// Create ConfigurationSection
        /// </summary>
        /// <typeparam name="T">Descendants of ConfigurationSection</typeparam>
        /// <param name="configPath">config path ex: 'shop/shopProperties'</param>
        /// <returns></returns>
        public static T GetOrCreateSection<T>(string configPath) where T : ConfigurationSection, new()
        {
            T section = null;
            try
            {
                section = ConfigurationManager.GetSection(configPath) as T;
            }
            catch (ConfigurationErrorsException)
            {
                section = CreateSection<T>(configPath);
            }

            if (section == null)
            {
                throw new NullReferenceException("There was an error creating the configuration section");
            }

            return section;
        }

        private static T CreateSection<T>(string configPath) where T : ConfigurationSection, new()
        {
            var section = new T();
            var fileName = PathUtil.Resolve(GetSectionPath(configPath));

            try
            {
                if (!File.Exists(fileName))
                {
                    int indexSlash = configPath.LastIndexOf('/');
                    var nameSection = configPath.Substring(indexSlash + 1);
                    var info = typeof(ConfigurationSection).GetMethod("SerializeSection", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    var serializedShopSection = info.Invoke(section, new object[] { null, nameSection, ConfigurationSaveMode.Full }).ToString();
                    var doc = new XDocument(new XDeclaration("1.0", "utf-8", null),
                        XElement.Parse(serializedShopSection));

                    doc.Save(fileName);
                }
            }
            catch (Exception e)
            {
                Log.LogCritical("Create section", e);

                throw;
            }

            return ConfigurationManager.GetSection(configPath) as T;
        }

        private static string GetSectionPath(string configPath)
        {
            try
            {
                var doc = XDocument.Load(PathUtil.Resolve("~/web.config"));
                var confPath = doc.XPathSelectElement("/configuration/" + configPath).FirstAttribute.Value;

                return Path.Combine("~", confPath);
            }
            catch (Exception e)
            {
                Log.LogCritical("Get section path", e);

                throw;
            }
        }
    }
}
