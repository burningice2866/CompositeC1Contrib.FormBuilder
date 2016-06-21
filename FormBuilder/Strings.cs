using Composite.Core.ResourceSystem;

namespace CompositeC1Contrib.FormBuilder
{
    public class Strings
    {
        public static string GetLocalized(string text)
        {
            return text.Contains("${") ? StringResourceSystemFacade.ParseString(text) : text;
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
