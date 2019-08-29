using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using DotNetHelper.FastMember.Extension.Helpers;
using DotNetHelper.FastMember.Extension.Models;

namespace DotNetHelper.FastMember.Extension.Tests.Extension
{
    internal static class ExtDataReader
    {



        ///// <summary>
        ///// Reads all available bytes from reader
        ///// </summary>
        ///// <param name="reader"></param>
        ///// <param name="ordinal"></param>
        ///// <returns></returns>
        //public static byte[] GetBytes(this IDataReader reader, int ordinal)
        //{
        //    byte[] result = null;

        //    if (!reader.IsDBNull(ordinal))
        //    {
        //        var size = reader.GetBytes(ordinal, 0, null, 0, 0); //get the length of data 
        //        result = new byte[size];
        //        var bufferSize = 1024;
        //        long bytesRead = 0;
        //        var curPos = 0;
        //        while (bytesRead < size)
        //        {
        //            bytesRead += reader.GetBytes(ordinal, curPos, result, curPos, bufferSize);
        //            curPos += bufferSize;
        //        }
        //    }

        //    return result;
        //}


        private static Dictionary<string, int> GetColumnDefinitionFromIDataReader(IDataReader reader)
        {
            var readerFieldLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase); // store name and ordinal
            for (var i = 0; i < reader.FieldCount; i++)
            {
                readerFieldLookup.Add(reader.GetName(i), i);
            }
            return readerFieldLookup;
        }

        public static bool? HasRows(this IDataReader reader)
        {
            if (reader is DbDataReader a)
                return a.HasRows;
            return null;
        }

        /// <summary>
        /// Reads the current row in the dataReader and map it to  a instance of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="readerFieldLookup">A dictionary that contains the columns definition</param>
        /// <param name="xmlDeserializer">A </param>
        /// <param name="jsonDeserializer"></param>
        /// <param name="csvDeserializer"></param>
        /// <returns></returns>
        private static T DataRecordToT<T>(IDataReader reader, Dictionary<string, int> readerFieldLookup, Func<string, Type, object> xmlDeserializer, Func<string, Type, object> jsonDeserializer, Func<string, Type, object> csvDeserializer) where T : class
        {
            var tType = typeof(T);
            if (tType == typeof(string))
            {
                if (reader.IsDBNull(0))
                {
                    return null;
                }
                else
                {
                    var value = reader.GetValue(0).ToString();
                    return value as T;
                }
            }

            if (tType.IsTypeDynamic() || tType == typeof(object))
            {
                var dynamicInstance = new ExpandoObject();
                readerFieldLookup.ForEach(delegate (KeyValuePair<string, int> pair)
                {
                    var value = reader.GetValue(pair.Value);
                    ExtFastMember.SetMemberValue(dynamicInstance, pair.Key, value == DBNull.Value ? null : value);
                });
                return dynamicInstance as T;
            }
            else
            {
                var newInstance = New<T>.Instance();
                var memberWrappers = ExtFastMember.GetMemberWrappers<T>(true);
                readerFieldLookup.ForEach(delegate (KeyValuePair<string, int> pair)
                {
                    var memberWrapper = memberWrappers.FirstOrDefault(w => w.Name == pair.Key);
                    if (memberWrapper != null)
                    {
                        var value = reader.GetValue(pair.Value);
                        try
                        {
                            ExtFastMember.SetMemberValue(newInstance, memberWrapper.Name, value);
                        }
                        catch (InvalidOperationException) { } // These are properties or field without a setter
                        catch (ArgumentOutOfRangeException) { }

                    }
                    else
                    {
                        // Datareader return a columns that doesn't exist in type so skip it
                    }

                });
                return newInstance;
            }

        }
        /// <summary>
        /// Maps the IDataReder to a list of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="xmlDeserializer">If the type of T have a property that is marked as xml & is stored in the database as xml. this function will execute to deserialize the value</param>
        /// <param name="jsonDeserializer">If the type of T have a property that is marked as json & is stored in the database as json. this function will execute to deserialize the value</param>
        /// <param name="csvDeserializer">If the type of T have a property that is marked as csv & is stored in the database as csv. this function will execute to deserialize the value</param>
        /// <returns></returns>
        public static List<T> MapToList<T>(this IDataReader reader, Func<string, Type, object> xmlDeserializer, Func<string, Type, object> jsonDeserializer, Func<string, Type, object> csvDeserializer) where T : class
        {
            if (reader == null || reader.IsClosed)
            {
                return new List<T>() { };
            }
            var pocoList = new List<T>() { };

            var readerFieldLookup = GetColumnDefinitionFromIDataReader(reader);

            while (reader.Read())
            {
                pocoList.Add(DataRecordToT<T>(reader, readerFieldLookup, xmlDeserializer, jsonDeserializer, csvDeserializer));
            }
            reader.Close();
            reader.Dispose();
            return pocoList;
        }



        /// <summary>
        /// Maps the IDataReder to a list of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>

        /// <returns></returns>
        public static List<T> MapToList<T>(this IDataReader reader) where T : class
        {
            return reader.MapToList<T>(null, null, null);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="xmlDeserializer">If the type of T have a property that is marked as xml & is stored in the database as xml. this function will execute to deserialize the value</param>
        /// <param name="jsonDeserializer">If the type of T have a property that is marked as json & is stored in the database as json. this function will execute to deserialize the value</param>
        /// <param name="csvDeserializer">If the type of T have a property that is marked as csv & is stored in the database as csv. this function will execute to deserialize the value</param>
        /// <returns></returns>
        public static T MapTo<T>(this IDataReader reader, Func<string, Type, object> xmlDeserializer, Func<string, Type, object> jsonDeserializer, Func<string, Type, object> csvDeserializer) where T : class
        {
            if (reader == null || reader.IsClosed)
            {
                return null;
            }
            var readerFieldLookup = GetColumnDefinitionFromIDataReader(reader);
            while (reader.Read())
            {
                return (DataRecordToT<T>(reader, readerFieldLookup, xmlDeserializer, jsonDeserializer, csvDeserializer));
            }
            return null;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static T MapTo<T>(this IDataReader reader) where T : class
        {
            return reader.MapTo<T>(null, null, null);
        }








        public static DataTable MapToDataTable<T>(this IEnumerable<T> source) where T : class
        {
            return MapToDataTable(source, null);
        }
        public static DataTable MapToDataTable<T>(this IEnumerable<T> source, string tableName) where T : class
        {

            var dt = new DataTable();
            if (source.Count() == 0)
            {
                if (source is IEnumerable<IDynamicMetaObjectProvider>)
                    return dt;
            }
            List<MemberWrapper> members;
            if (source is IEnumerable<IDynamicMetaObjectProvider> listOfDynamicObjects)
            {
                members = ExtFastMember.GetMemberWrappers(listOfDynamicObjects.First()).AsList();
            }
            else
            {
                members = ExtFastMember.GetMemberWrappers<T>(true).AsList();
            }

            var keyColumns = new List<DataColumn>() { };
            members.ForEach(delegate (MemberWrapper member)
            {
                var dc = new DataColumn(member.Name, member.Type.IsNullable().underlyingType); // datacolumn doesn't support nullable type so use underlying type or default

                dt.Columns.Add(dc);
            });
            dt.PrimaryKey = keyColumns.ToArray();

            source.AsList().ForEach(delegate (T obj)
            {
                var row = dt.NewRow();
                members.ForEach(w => row[w.Name] = w.GetValue(obj) ?? DBNull.Value);
                dt.Rows.Add(row);
            });

            dt.TableName = tableName;
            return dt;
        }

    }
}
