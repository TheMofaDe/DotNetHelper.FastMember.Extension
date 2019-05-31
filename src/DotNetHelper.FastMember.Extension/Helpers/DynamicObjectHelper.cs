using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using DotNetHelper.FastMember.Extension.Extension;

namespace DotNetHelper.FastMember.Extension.Helpers
{
    public static class DynamicObjectHelper
    {

        public static void AddProperty(ExpandoObject expandoObject, string propertyName, object value)
        {
            var x = expandoObject as IDictionary<string, object>;
            if (x.Keys.Contains(propertyName))
            {
                x[propertyName] = value;
            }
            x.Add(propertyName, value);
        }

        public static void RemoveProperty(ExpandoObject expandoObject, string propertyName)
        {
            // ReSharper disable once UsePatternMatching
            var x = expandoObject as IDictionary<string, object>;
            if (x == null) return;
            if (x.Keys.Contains(propertyName))
            {
                x.Remove(propertyName);
            }
        }

        public static void AddPropertyChangedHander(ExpandoObject expandoObject, PropertyChangedEventHandler action)
        {
            ((INotifyPropertyChanged)expandoObject).PropertyChanged += action;
        }

        public static IDataReader ToDataReader(ExpandoObject expandoObject)
        {
            var x = expandoObject as IDictionary<string, object>;
            return x.MapToDataTable().CreateDataReader();
        }


        public static DataTable ToDataTable(ExpandoObject expandoObject)
        {
            var x = expandoObject as IDictionary<string, object>;
            return x.MapToDataTable();
        }

        /// <summary>
        /// return a dictionary of properties and values
        /// </summary>
        /// <param name="expandoObject"></param>
        /// <returns></returns>
        public static IDictionary<string, object> GetProperties(ExpandoObject expandoObject)
        {
            var x = expandoObject as IDictionary<string, object>;
            return x;
        }


        public static object GetPropertyValue(ExpandoObject expandoObject, string propertyName, bool searchAllChildrens = true)
        {
            propertyName.IsNullThrow(nameof(propertyName));

            var props = GetProperties(expandoObject);
            if (props != null)
            {
                if (props.ContainsKey(propertyName))
                {
#if NETFRAMEWORK
                    return props.GetValueOrDefault(propertyName);
#else
                    return props.GetValueOrDefaultValue(propertyName);
#endif
                }
                foreach (var x in props)
                {
                    if (x.Value is ExpandoObject x1)
                    {
                        var z = GetPropertyValue(x1, propertyName);
                        if (z == null) continue;
                        return z;
                    }
                }
            }
            return null;
        }


    }
}
