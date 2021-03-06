﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using DotNetHelper.FastMember.Extension.Extension;
using DotNetHelper.FastMember.Extension.Helpers;
using DotNetHelper.FastMember.Extension.Models;
using FastMember;

namespace DotNetHelper.FastMember.Extension
{
    // https://stackoverflow.com/questions/315146/anonymous-types-are-there-any-distingushing-characteristics
    public static class ExtFastMember
    {
        public static bool UseRuntimeReflection { get; set; } = OSHelper.IsRunningOnIOS;
        private static Type GuidType { get; } = typeof(Guid);
        private static Type GuidTypeNullable { get; } = typeof(Guid?);
        private static Type DateTimeOffsetType { get; } = typeof(DateTimeOffset);
        private static Type DateTimeOffsetTypeNullable { get; } = typeof(DateTimeOffset?);
        private static Type TimeSpanType { get; } = typeof(TimeSpan);
        private static Type TimeSpanTypeNullable { get; } = typeof(TimeSpan?);

        private static IDictionary<string, List<MemberWrapper>> Lookup { get; } = new Dictionary<string, List<MemberWrapper>>();
        //    private static IDictionary<string, List<MemberWrapper>> Lookup { get; } = new ConcurrentDictionary<string, List<MemberWrapper>>();

        private static object Lock { get; } = new object();


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="includeNonPublicAccessor"></param>
        /// <returns>A List Of Advance Members Of T</returns>
        public static List<MemberWrapper> GetMemberWrappers(Type type, bool includeNonPublicAccessor)
        {
            type.IsNullThrow(nameof(type));
            // ReSharper disable once AssignNullToNotNullAttribute
            var key = $"{type.FullName}{includeNonPublicAccessor}";
            lock (Lock)
            {
                // https://stackoverflow.com/questions/9382681/what-is-more-efficient-dictionary-trygetvalue-or-containskeyitem
                var membersAlreadyExist = Lookup.TryGetValue(key, out var cachedMembers);
                if (membersAlreadyExist)
                    return cachedMembers;

                var list = new List<MemberWrapper>() { };
                if (UseRuntimeReflection)
                {
                    type.GetProperties().ForEach(delegate (PropertyInfo property)
                    {
                        var advance = new MemberWrapper(property) { };
                        list.Add(advance);
                    });
                }
                else
                {
                    var accessor = TypeAccessor.Create(type, includeNonPublicAccessor);
                    var set = accessor.GetMembers();
                    for (var index = 0; index < set.Count; index++)
                    {
	                    var member = set[index];
	                    var advance = new MemberWrapper(member) { };
	                    list.Add(advance);
                    }
                }
                Lookup.Add(key, list);
                return list;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dynamicObject"></param>
        /// <returns>A List Of Advance Members Of T</returns>
        public static List<MemberWrapper> GetMemberWrappers(IDynamicMetaObjectProvider dynamicObject)
        {
            var list = new List<MemberWrapper>() { };

            var dynamicObjectHelper = new DynamicObjectHelper();
            dynamicObjectHelper.GetDynamicMemberNameAndValues(dynamicObject).ForEach(
                delegate (KeyValuePair<string, object> pair)
                {
                    var advance = new MemberWrapper(pair.Key, pair.Value == null ? typeof(object) : pair.Value.GetType()) { };
                    list.Add(advance);
                });

            return list;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="includeNonPublicAccessor"></param>
        /// <returns>A List Of Advance Members Of T</returns>
        /// <exception cref="InvalidOperationException"> This method doesn't support dynamic types. Please use</exception>
        public static List<MemberWrapper> GetMemberWrappers<T>(bool includeNonPublicAccessor) where T : class
        {
            if (typeof(T).IsTypeDynamic())
                throw new InvalidOperationException("Method : GetAdvanceMembers doesn't support dynamic objects please use GetDynamicAdvanceMembers instead.");

            var type = typeof(T);
            // ReSharper disable once AssignNullToNotNullAttribute
            var key = $"{type.FullName}{includeNonPublicAccessor}";
            lock (Lock)
            {
                //if (Lookup.TryGetValue(key, out var cachedMembers))
                var membersAlreadyExist = Lookup.TryGetValue(key, out var cachedMembers);
                if (membersAlreadyExist)
                    return cachedMembers;

                var list = new List<MemberWrapper>() { };
                var accessor = TypeAccessor.Create(type, includeNonPublicAccessor);
                var set = accessor.GetMembers();
                for (var index = 0; index < set.Count; index++)
                {
                    var member = set[index];
                    var advance = new MemberWrapper(member) { };
                    list.Add(advance);
                }


                Lookup.Add(key, list);
                return list;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>A List Of Advance Members Of T</returns>
        public static List<MemberWrapper> GetMemberWrappers<T>(T dynamicObject) where T : IDynamicMetaObjectProvider
        {
            var list = new List<MemberWrapper>() { };

            var dynamicObjectHelper = new DynamicObjectHelper();
            dynamicObjectHelper.GetDynamicMemberNameAndValues(dynamicObject).ForEach(
                delegate (KeyValuePair<string, object> pair)
                {
                    var advance = new MemberWrapper(pair.Key, pair.Value == null ? typeof(object) : pair.Value.GetType()) { };
                    list.Add(advance);
                });

            return list;
        }


        public static void SetMemberValue<T>(T poco, string propertyName, object value)
        {
            poco.IsNullThrow(nameof(poco));
            propertyName.IsNullThrow(nameof(propertyName));

            if (poco is IDynamicMetaObjectProvider dynamicObject)
            {
                new DynamicObjectHelper().TrySetMember(dynamicObject, propertyName, value);
                return;
            }
            var accessor = TypeAccessor.Create(typeof(T), true);
            var members = accessor.GetMembers().ToList();
            var propertyMember = members.FirstOrDefault(a => string.Equals(a.Name, propertyName, StringComparison.CurrentCultureIgnoreCase));
            if (propertyMember == null) throw new InvalidOperationException($"No property found named '{propertyName}' of type {typeof(T).FullName}");


            var needToBeType = members.First(m => m.Name == propertyName).Type;

            void SetValue(object propertyValue)
            {
                try
                {
                    accessor[poco, propertyName] = propertyValue;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    if (typeof(T).IsTypeAnonymousType())
                    {
                        throw new InvalidOperationException("Anonymous object are meant to hold values and is not mutable ", e);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Is the property {propertyName} of type {typeof(T).FullName} missing a setter ??? check inner exception for more detail ", e);

                    }
                }
            }
            if (value == null || value == DBNull.Value)
            {
                SetValue(null);
                return;
            }

            var isDifferentType = value.GetType() != needToBeType;
            if (isDifferentType)
            {
                if (needToBeType == DateTimeOffsetType || needToBeType == GuidType || needToBeType == TimeSpanType || needToBeType == DateTimeOffsetTypeNullable || needToBeType == GuidTypeNullable || needToBeType == TimeSpanTypeNullable)
                {
                    value = TypeDescriptor.GetConverter(needToBeType).ConvertFrom(value);
                }

                else
                {
                    if (needToBeType != typeof(object) && needToBeType != typeof(string))
                        value = needToBeType.IsEnum
                            ? Enum.Parse(needToBeType.IsNullable().underlyingType, value.ToString(), true)
                            : Convert.ChangeType(value, needToBeType.IsNullable().underlyingType, null);
                }
            }

            if (isDifferentType && needToBeType == typeof(string))
            {
                SetValue(value.ToString());
            }
            else
            {
                SetValue(value);
            }


        }



        public static object GetMemberValue<T>(T instance, string key) where T : class
        {
            if (typeof(T).IsTypeDynamic())
                return GetMemberValue((IDynamicMetaObjectProvider)instance, key);
            return GetMemberWrappers<T>(true).FirstOrDefault(w => w.Name == key)?.GetValue(instance);
        }
        public static object GetMemberValue(IDynamicMetaObjectProvider instance, string key)
        {
            return GetMemberWrappers<IDynamicMetaObjectProvider>(instance).FirstOrDefault(w => w.Name == key)?.GetValue(instance);
        }

    }
}
