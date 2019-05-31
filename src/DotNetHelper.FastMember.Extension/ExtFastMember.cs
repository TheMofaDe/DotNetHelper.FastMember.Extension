using System;
using System.Collections.Generic;
using System.Dynamic;
using DotNetHelper.FastMember.Extension.Extension;
using DotNetHelper.FastMember.Extension.Helpers;
using DotNetHelper.FastMember.Extension.Models;
using FastMember;

namespace DotNetHelper.FastMember.Extension
{

    public static class ExtFastMember
    {

        private static IDictionary<string, List<AdvanceMember>> Lookup { get; } = new Dictionary<string, List<AdvanceMember>>();

        private static object Lock { get; } = new object();




        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poco">If Null Default Value Will Be Used For Members</param>
        /// <returns>A List Of Advance Members Of T</returns>
        public static List<DynamicAdvanceMember> GetDynamicAdvanceMembers<T>(T poco) where T : IDynamicMetaObjectProvider
        {
            poco.IsNullThrow(nameof(poco));

            var list = new List<DynamicAdvanceMember>() { };
            var props = DynamicObjectHelper.GetProperties(poco as ExpandoObject);

            props.ForEach(delegate (KeyValuePair<string, object> pair)
            {
                var keyType = pair.Value == null ? typeof(object) : pair.Value.GetType();
                list.Add(new DynamicAdvanceMember(pair.Key,keyType));
            });
            return list;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="includeNonPublicAccessor"></param>
        /// <returns>A List Of Advance Members Of T</returns>
        public static List<AdvanceMember> GetAdvanceMembers(Type type, bool includeNonPublicAccessor = true)
        {
            type.IsNullThrow(nameof(type));
            // ReSharper disable once AssignNullToNotNullAttribute
            var key = type.FullName + includeNonPublicAccessor;
            lock (Lock)
            {
                //if (Lookup.TryGetValue(key, out var cachedMembers))
                if (Lookup.ContainsKey(key))
                {
                    return Lookup[key];
                }

                var list = new List<AdvanceMember>() { };
                var accessor = TypeAccessor.Create(type, includeNonPublicAccessor);
                accessor.GetMembers().AsList().ForEach(delegate (Member member)
                {
                    var advance = new AdvanceMember(member) { };
                    list.Add(advance);
                });

                Lookup.Add(key, list);
                return list;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poco">If Null Default Value Will Be Used For Members</param>
        /// <param name="includeNonPublicAccessor"></param>
        /// <returns>A List Of Advance Members Of T</returns>
        public static List<AdvanceMember> GetAdvanceMembers<T>(bool includeNonPublicAccessor = true) where T : class
        {
            if (typeof(T) == typeof(ExpandoObject) || typeof(IDynamicMetaObjectProvider).IsAssignableFrom(typeof(T)))
                throw new InvalidOperationException("Method : GetAdvanceMembers doesn't support dynamic objects please use GetDynamicAdvanceMembers instead.");

            var type = typeof(T);
            // ReSharper disable once AssignNullToNotNullAttribute
            var key = type.FullName + includeNonPublicAccessor;
            lock (Lock)
            {
                //if (Lookup.TryGetValue(key, out var cachedMembers))
                if (Lookup.ContainsKey(key))
                {
                    return Lookup[key];
                }

                var list = new List<AdvanceMember>() { };
                var accessor = TypeAccessor.Create(type, includeNonPublicAccessor);
                accessor.GetMembers().AsList().ForEach(delegate (Member member)
                {
                    var advance = new AdvanceMember(member) { };
                    list.Add(advance);
                });

                Lookup.Add(key, list);
                return list;
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="poco">If Null Default Value Will Be Used For Members</param>
        ///// <param name="includeNonPublicAccessor"></param>
        ///// <returns>A List Of Advance Members Of T</returns>
        //public static List<AdvanceMember> GetAdvanceMembers<T>( bool includeNonPublicAccessor = true) where T : class
        //{
        //    if (typeof(T) == typeof(ExpandoObject) || typeof(IDynamicMetaObjectProvider).IsAssignableFrom(typeof(T)))
        //        throw new InvalidOperationException("Method : GetAdvanceMembers doesn't support dynamic objects please use GetDynamicAdvanceMembers instead.");


        //    return GetAdvanceMembers<T>(includeNonPublicAccessor);
        //}









    }
}