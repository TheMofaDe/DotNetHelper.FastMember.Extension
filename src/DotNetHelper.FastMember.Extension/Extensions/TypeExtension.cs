using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DotNetHelper.FastMember.Extension.Extension
{
    internal static class TypeExtension
    {

        public static bool HasDefaultConstructor(this Type t) => t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static (bool isNullableT,Type underlyingType) IsNullable (this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var childType = Nullable.GetUnderlyingType(type);
                return (true, childType);
            }
            return (false, type);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsTypeAnIEnumerable(this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static bool IsTypeDynamic(this Type type)
        {
            return typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type);
        }

        public static bool IsTypeAnonymousType(this Type type)
        {
            // https://stackoverflow.com/questions/2483023/how-to-test-if-a-type-is-anonymous
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                   && type.IsGenericType && type.Name.Contains("AnonymousType")
                   && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                   && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }
        public static Type GetEnumerableItemType(this Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            Type elementType = null;

            if (type == typeof(IEnumerable))
            {
                elementType = typeof(object);
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                elementType = type.GetGenericArguments()[0];
            }
            else
            {
                foreach (var interfaceType in type.GetInterfaces())
                {
                    if (interfaceType == typeof(IEnumerable))
                    {
                        elementType = typeof(object);
                    }
                    else if (interfaceType.IsGenericType &&
                             interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        elementType = interfaceType.GetGenericArguments()[0];
                        break;
                    }
                }
            }


            return elementType;
        }



        public static readonly Dictionary<Type, object> CommonTypeDictionary = new Dictionary<Type, object>
        {
#pragma warning disable IDE0034 // DO NOT REMOVE THIS CODE Simplify 'default' expression - default causes default(object)
            { typeof(int), default(int) },
            { typeof(Guid), default(Guid) },
            { typeof(DateTime), default(DateTime) },
            { typeof(DateTimeOffset), default(DateTimeOffset) },
            { typeof(long), default(long) },
            { typeof(bool), default(bool) },
            { typeof(double), default(double) },
            { typeof(short), default(short) },
            { typeof(float), default(float) },
            { typeof(byte), default(byte) },
            { typeof(char), default(char) },
            { typeof(uint), default(uint) },
            { typeof(ushort), default(ushort) },
            { typeof(ulong), default(ulong) },
            { typeof(sbyte), default(sbyte) }
#pragma warning restore IDE0034 // Simplify 'default' expression
        };
        public static object GetDefaultValue(this Type type)
        {
            if (!type.GetTypeInfo().IsValueType)
            {
                return null;
            }

            // A bit of perf code to avoid calling Activator.CreateInstance for common types and
            // to avoid boxing on every call. This is about 50% faster than just calling CreateInstance
            // for all value types.
            return CommonTypeDictionary.TryGetValue(type, out var value) ? value : Activator.CreateInstance(type);
        }





    }
}
