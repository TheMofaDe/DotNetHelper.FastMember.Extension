using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using DotNetHelper.FastMember.Extension.Extension;
using DotNetHelper.FastMember.Extension.Helpers;
using DotNetHelper.FastMember.Extension.Interface;
using DotNetHelper.FastMember.Extension.Models;
using FastMember;

namespace DotNetHelper.FastMember.Extension
{

    public static class ExtFastMember
    {

      //  public static IDictionary<T, List<AdvanceMember>> AttributeLookup { get; } = new Dictionary<string, List<AdvanceMember>>();
        private static IDictionary<string, List<MemberWrapper>> Lookup { get; } = new Dictionary<string, List<MemberWrapper>>();

        private static object Lock { get; } = new object();




        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poco">If Null Default Value Will Be Used For Members</param>
        /// <returns>A List Of Advance Members Of T</returns>
        public static List<DynamicMember> GetDynamicMembers<T>(T poco) where T : IDynamicMetaObjectProvider
        {
            poco.IsNullThrow(nameof(poco));

            var list = new List<DynamicMember>() { };
            var props = DynamicObjectHelper.GetProperties(poco as ExpandoObject);

            props.ForEach(delegate (KeyValuePair<string, object> pair)
            {
                var keyType = pair.Value == null ? typeof(object) : pair.Value.GetType();
                list.Add(new DynamicMember(pair.Key,keyType));
            });
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="includeNonPublicAccessor"></param>
        /// <returns>A List Of Advance Members Of T</returns>
        public static List<MemberWrapper> GetMemberWrappers(Type type, bool includeNonPublicAccessor = true)
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

                var list = new List<MemberWrapper>() { };
                var accessor = TypeAccessor.Create(type, includeNonPublicAccessor);
                accessor.GetMembers().AsList().ForEach(delegate (Member member)
                {
                    var advance = new MemberWrapper(member) { };
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
        public static List<MemberWrapper> GetMemberWrappers<T>(bool includeNonPublicAccessor = true) where T : class
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

                var list = new List<MemberWrapper>() { };
                var accessor = TypeAccessor.Create(type, includeNonPublicAccessor);
                accessor.GetMembers().AsList().ForEach(delegate (Member member)
                {
                    var advance = new MemberWrapper(member) { };
                    list.Add(advance);
                });

                Lookup.Add(key, list);
                return list;
            }
        }

   


        public static void SetMemberValue<T>(T poco, string propertyName, object value)
        {
            poco.IsNullThrow(nameof(poco));
            propertyName.IsNullThrow(nameof(propertyName));

            var accessor = TypeAccessor.Create(typeof(T), true);
            var members = accessor.GetMembers().ToList();
            if (string.IsNullOrEmpty(propertyName) || !accessor.GetMembers().ToList().Exists(a => string.Equals(a.Name, propertyName, StringComparison.CurrentCultureIgnoreCase))) throw new InvalidOperationException("SetMemberValue Method Can't Work If You Pass It Null Object Or Invalid Property Name");

            var needToBeType = members.First(m => m.Name == propertyName).Type;

            if (value == null)
            {
                accessor[poco, propertyName] = null;
                return;
            }
            if (value.GetType() != needToBeType)
            {

                if (needToBeType == typeof(DateTimeOffset) || needToBeType == typeof(DateTimeOffset?))
                {
                    value = TypeDescriptor.GetConverter(needToBeType).ConvertFrom(value);
                }
                else
                {
                    value = needToBeType.IsEnum
                        ? System.Enum.Parse(needToBeType.IsNullable().underlyingType, value.ToString(), true)
                        : Convert.ChangeType(value, needToBeType.IsNullable().underlyingType, null);
                }

            }
            accessor[poco, propertyName] = value;
        }




    }
}