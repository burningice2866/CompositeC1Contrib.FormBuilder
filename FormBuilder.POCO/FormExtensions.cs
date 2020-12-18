using System;

namespace CompositeC1Contrib.FormBuilder
{
    public static class FormExtensions
    {
        public static T ToPoco<T>(this Form form) where T : class
        {
            if (!form.FormData.TryGetValue("PocoInstance", out var instance))
            {
                throw new ArgumentException("No Poco associated with the form");
            }

            var poco = instance as T;
            if (poco == null)
            {
                throw new InvalidOperationException("Poco was not of the expected type");
            }

            return poco;
        }
    }
}
