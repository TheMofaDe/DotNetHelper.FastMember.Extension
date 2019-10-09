using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;
using DotNetHelper.FastMember.Extension.Extension;
using DotNetHelper.FastMember.Extension.Helpers;
using FastMember;

namespace DotNetHelper.FastMember.Extension.Models
{
    public class MemberWrapper
    {
        private IDictionary<Type, object> CustomAttributeLookup { get; set; } = new ConcurrentDictionary<Type, object>();
        private Member Member { get; }
        public string Name { get; }
        public Type Type { get; }
        public bool CanRead { get; }
        public bool CanWrite { get; }
        public bool IsADynamicMember { get; } = false;

        internal MemberWrapper(Member member)
        {
            Member = member;
            Name = Member.Name;
            Type = Member.Type;
            try
            {
                CanRead = Member.CanRead;
            }
            catch (NotSupportedException)
            {
                // we can still get and set the value of fields and if its read-only your compiler will stop it
                CanRead = member.GetMemberInfo().MemberType == MemberTypes.Field;
            }
            try
            {
                CanWrite = member.GetMemberInfo().MemberType == MemberTypes.Field;
            }
            catch (NotSupportedException)
            {
                CanWrite = false;
            }

        }


        /// <summary>
        /// Only use this for IDynamicMetaObjectProvider types. Initializes a new instance of the <see cref="MemberWrapper"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        internal MemberWrapper(string name, Type type)
        {
            Name = name;
            Type = type;
            CanRead = true;
            CanWrite = true;
            IsADynamicMember = true;
        }


        public T GetCustomAttribute<T>() where T : Attribute
        {
            if (IsADynamicMember) return null; // 
            var hasRecord = CustomAttributeLookup.TryGetValue(typeof(T), out var value);
            if (hasRecord)
            {
                // ReSharper disable once UseNullPropagation
                if (value == null) return null;
                return (T)value;
            }
            else
            {
                if (Member.IsDefined(typeof(T)))
                {
                    var customAttr = Member.GetMemberAttribute<T>(); // TODO :: CHECK TO SEE IF THIS RETURN NULL IF NOT DEFINED IF SO REMOVE THE CHECK ALSO WRITE UNIT TO ENSURE WE ARE NOTIFIED WHEN THIS FUNCTIONALITY CHANGES
                    CustomAttributeLookup.Add(typeof(T), customAttr);
                    return customAttr;
                }
            }
            return null;
        }

        public MemberInfo GetMemberInfo()
        {
            if (IsADynamicMember) throw new InvalidOperationException("Can't retrieve MemberInfo from dynamic objects");
            return Member.GetMemberInfo();
        }

        public object GetValue(object instanceOfObject)
        {
            if (instanceOfObject is IDynamicMetaObjectProvider dynamicInstance)
            {
                var helper = new DynamicObjectHelper();
                helper.TryGetMember(dynamicInstance, Name, out var value);
                return value;
            }
            var accessor = TypeAccessor.Create(instanceOfObject.GetType(), true);
            return accessor[instanceOfObject, Name];
        }
        public object GetValue(object instanceOfObject, TypeAccessor accessor)
        {
            if (instanceOfObject is IDynamicMetaObjectProvider dynamicInstance)
            {
                var helper = new DynamicObjectHelper();
                helper.TryGetMember(dynamicInstance, Name, out var value);
                return value;
            }
            accessor.IsNullThrow(nameof(accessor)); // TODO :: UNIT TEST ENSURE IT THROWS
            return accessor[instanceOfObject, Name];
        }

        public object GetValue<T>(T instanceOfObject) where T : class
        {
            if (instanceOfObject is IDynamicMetaObjectProvider dynamicInstance)
            {
                var helper = new DynamicObjectHelper();
                helper.TryGetMember(dynamicInstance, Name, out var value);
                return value;
            }
            var accessor = TypeAccessor.Create(instanceOfObject.GetType(), true);
            return accessor[instanceOfObject, Name];
        }
        public object GetValue<T>(T instanceOfObject, TypeAccessor accessor) where T : class
        {
            if (instanceOfObject is IDynamicMetaObjectProvider dynamicInstance)
            {
                var helper = new DynamicObjectHelper();
                helper.TryGetMember(dynamicInstance, Name, out var value);
                return value;
            }
            return accessor[instanceOfObject, Name];
        }

        public void SetMemberValue<T>(T instanceOfObject, object value)
        {
            ExtFastMember.SetMemberValue(instanceOfObject, Name, value);
        }



    }




}
