using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using Composite.Core.ResourceSystem;

namespace CompositeC1Contrib.FormBuilder
{
    public class Strings
    {
        private static Regex _t = new Regex("T\\((.+)\\)", RegexOptions.Compiled);

        public static string GetLocalized(string text)
        {
            if (text.Contains("T("))
            {
                var match = _t.Match(text);
                if (match.Success)
                {
                    var key = match.Groups[1].Value;
                    var localized = HttpContext.GetGlobalResourceObject(String.Empty, key) as string;
                    if (localized != null)
                    {
                        text = text.Remove(match.Index, match.Length).Insert(match.Index, localized);
                    }
                }
            }

            if (text.Contains("${"))
            {
                text = StringResourceSystemFacade.ParseString(text);
            }

            return text;
        }

        public static string GetLocalizedByKey(string key)
        {
            var localized = HttpContext.GetGlobalResourceObject(String.Empty, key) as string;
            if (localized != null && localized != key)
            {
                return localized;
            }

            key = String.Join(".", key.Split('.').Skip(1));

            return StringResourceSystemFacade.GetString("CompositeC1Contrib.FormBuilder", key);
        }

        public static bool IsEqual(object obj, string value)
        {
            if (obj is bool)
            {
                return bool.Parse(value) == (bool)obj;
            }

            return obj.ToString() == value;
        }
    }
}
