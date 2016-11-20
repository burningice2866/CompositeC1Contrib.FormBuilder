namespace CompositeC1Contrib.FormBuilder
{
    public static class StringExtensions
    {
        public static bool IsEqualTo(this string value, object obj)
        {
            if (obj is bool)
            {
                return bool.Parse(value) == (bool)obj;
            }

            return obj.ToString() == value;
        }
    }
}
