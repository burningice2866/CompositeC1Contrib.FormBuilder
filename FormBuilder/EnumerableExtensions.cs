using System.Collections.Generic;
using System.Linq;

namespace CompositeC1Contrib.FormBuilder
{
    public static class EnumerableExtensions
    {
        public static FormField Get(this IEnumerable<FormField> list, string name) => list.SingleOrDefault(f => f.Name == name);

        public static FormFieldModel Get(this IEnumerable<FormFieldModel> list, string name) => list.SingleOrDefault(f => f.Name == name);

        public static KeyValuePair<TKey, TValue> Get<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list, TKey key) => list.SingleOrDefault(f => f.Key.Equals(key));
    }
}
