using System.Globalization;
using System.Text.RegularExpressions;

using Composite.Core.ResourceSystem;

using CompositeC1Contrib.Localization;

namespace CompositeC1Contrib.FormBuilder
{
    public static class Localization
    {
        private static readonly Regex _t = new Regex("T\\((.+)\\)", RegexOptions.Compiled);

        public const string ResourceSet = "FormBuilder";
        public const string KeyPrefix = "Forms";

        public static string Localize(string text)
        {
            if (text.Contains("T("))
            {
                var match = _t.Match(text);
                if (match.Success)
                {
                    var key = match.Groups[1].Value;

                    var localized = C1Res.T(key);
                    if (localized != null)
                    {
                        return text.Remove(match.Index, match.Length).Insert(match.Index, localized);
                    }

                }
            }

            if (text.Contains("${"))
            {
                return StringResourceSystemFacade.ParseString(text);
            }

            return text;
        }

        public static string EvaluateT(FormFieldModel field, string setting, string defaultValue)
        {
            return EvaluateT(field.OwningForm, field.Name + "." + setting, defaultValue);
        }

        public static string EvaluateT(IModel form, string setting, string defaultValue)
        {
            var key = GenerateKey(form.Name, setting);

            return EvaluateT(key, defaultValue);
        }

        public static string EvaluateT(string key, string defaultValue)
        {
            var evaluated = T(key);
            if (evaluated != null)
            {
                return evaluated;
            }

            return defaultValue == null ? null : Localize(defaultValue);
        }

        public static string T(string key)
        {
            var culture = CultureInfo.CurrentUICulture;

            return T(key, culture);
        }

        public static string T(string key, CultureInfo culture)
        {
            var value = C1Res.GetResourceManager(ResourceSet).GetString(key, culture);
            if (value == null)
            {
                value = ResourceFacade.InternalResourceManager.GetString(key.Replace(".", "_"), culture);
            }

            return value;
        }

        public static string GenerateKey(string form) => KeyPrefix + "." + form;

        public static string GenerateKey(string form, string setting) => KeyPrefix + "." + form + "." + setting;
    }
}
