namespace CompositeC1Contrib.FormBuilder
{
    public static class StringExtensions
    {
        public static bool IsEqualTo(this string value, object obj)
        {
            if (obj is bool b)
            {
                return bool.Parse(value) == b;
            }

            return obj.ToString() == value;
        }
    }
}
