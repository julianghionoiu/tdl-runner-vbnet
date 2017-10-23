using System.Collections.Generic;

namespace BeFaster.Runner.Extensions
{
    internal static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) =>
            dictionary.ContainsKey(key) ? dictionary[key] : default(TValue);
    }
}
