using System.Globalization;
using System.Text.RegularExpressions;

using Composite.Core.ResourceSystem;

namespace CompositeC1Contrib.FormBuilder
{
    public static class Localization
    {
        private static readonly Regex _t = new Regex("T\\((.+)\\)", RegexOptions.Compiled);

        public static string Localize(string text)
        {
            if (text.Contains("T("))
            {
                var match = _t.Match(text);
                if (match.Success)
                {
                    var key = match.Groups[1].Value;

                    var localized = T(key);
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

        public static string EvaluateT(FormFieldModel field, string type, string defaultValue)
        {
            var key = "Forms." + field.OwningForm.Name + "." + field.Name + "." + type;

            return EvaluateT(key, defaultValue);
        }

        public static string EvaluateT(string key, string defaultValue)
        {
            var evaluated = T(key);
            if (evaluated != null)
            {
                return evaluated;
            }

            return defaultValue == null ? key : Localize(defaultValue);
        }

        public static string T(string key)
        {
            var culture = CultureInfo.CurrentUICulture;

            return T(key, culture);
        }

        public static string T(string key, CultureInfo culture)
        {
            var resourceManager = ResourceFacade.GetResourceManager();
            if (resourceManager == null)
            {
                return null;
            }

            var localized = resourceManager.GetString(key, culture);
            if (localized == null)
            {
                localized = ResourceFacade.DefaultResourceManager.GetString(key, culture);
            }

            return localized;
        }
    }
}
