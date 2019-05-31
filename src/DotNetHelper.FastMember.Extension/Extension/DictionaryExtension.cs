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

        public static DataTable MapToDataTable(this IDictionary<string, object> dictionary, string tableName = null)
        {
            var dataTable = new DataTable(tableName);
            for (var i = 0; i < dictionary.Keys.Count; i++)
            {
                dataTable.Columns.Add(dictionary.Keys.AsList()[i], dictionary.Values.AsList()[i].GetType());
            }
            var dataRow = dataTable.NewRow();
            dataRow.ItemArray = dictionary.Values.Select(a => a ?? DBNull.Value).ToArray();
            dataTable.Rows.Add(dataRow);
            return dataTable;
        }
    }
}
