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
	        var keyExists = dictionary.TryGetValue(key, out var value);
	        if (keyExists)
		        return value;
	        return defaultValue;
        }
#else
        public static V GetValueOrDefaultValue<K, V>(this IDictionary<K, V> dictionary, K key, V defaultValue = default)
        {
            var keyExists = dictionary.TryGetValue(key, out var value);
            if (keyExists)
                return value;
            return defaultValue;
        }

#endif


    }
}
