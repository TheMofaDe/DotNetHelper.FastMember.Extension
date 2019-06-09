using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DotNetHelper.FastMember.Extension.Extension
{
    internal static class DictionaryExtension
    {
#if NETFRAMEWORK
        public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dictionary, K key, V defaultValue = default)
        {
            if (dictionary.ContainsKey(key))
            {
                if (dictionary.TryGetValue(key, out var a))
                    return a;
            }
            return defaultValue;
        }
#else
        public static V GetValueOrDefaultValue<K, V>(this IDictionary<K, V> dictionary, K key, V defaultValue = default)
        {
            if (dictionary.ContainsKey(key))
            {
                if (dictionary.TryGetValue(key, out var a))
                    return a;
            }
            return defaultValue;
        }

#endif


    }
}
