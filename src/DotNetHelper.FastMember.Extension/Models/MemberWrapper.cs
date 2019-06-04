using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using DotNetHelper.FastMember.Extension.Extension;
using DotNetHelper.FastMember.Extension.Interface;
using FastMember;

namespace DotNetHelper.FastMember.Extension.Models
{
    public class MemberWrapper : IMember
    {
        private IDictionary<Type, object> CustomAttributeLookup { get; set; } = new ConcurrentDictionary<Type, object>();
        internal Member Member { get; }
        public string Name { get; }
        public Type Type { get; }
        public bool CanRead { get; }
        public bool CanWrite { get; }

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
                // we can still get and set the value of fields and if its read-only you compiler will stop it
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


        public T GetCustomAttribute<T>() where T : Attribute
        {
            var hasRecord = CustomAttributeLookup.TryGetValue(typeof(T), out var value);
            if (hasRecord)
            {
                // ReSharper disable once UseNullPropagation
                if (value == null) return null; 
                return (T) value;
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
            return Member.GetMemberInfo();
        }

        public object GetValue(object instanceOfObject)
        {
            var accessor = TypeAccessor.Create(instanceOfObject.GetType(), true);
            return accessor[instanceOfObject, Member.Name];
        }
        public object GetValue(object instanceOfObject, TypeAccessor accessor)
        {
            return accessor[instanceOfObject, Member.Name];
        }

        public object GetValue<T>(T instanceOfObject) where T : class
        {
            var accessor = TypeAccessor.Create(typeof(T), true);
            return accessor[instanceOfObject, Member.Name];
        }
        public object GetValue<T>(T instanceOfObject, TypeAccessor accessor) where T : class
        {
            return accessor[instanceOfObject, Member.Name];
        }

        public void SetMemberValue<T>(T instanceOfObject, object value)
        {
            instanceOfObject.IsNullThrow(nameof(instanceOfObject));
            var accessor = TypeAccessor.Create(typeof(T), true);

            if (value == null)
            {
                try
                {
                    accessor[instanceOfObject, Member.Name] = null;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    if (typeof(T).IsTypeAnonymousType())
                    {
                        throw new InvalidOperationException("Anonymous object are meant to hold values and is not mutable ", e);
                    }
                    else
                    {
                        throw;
                    }
                }

                return;
            }
            if (value.GetType() != Member.Type) // TODO :: UNIT TEST FOR EVERY SINGLE SYSTEM TYPE
            {
                if (Member.Type == typeof(DateTimeOffset) || Member.Type == typeof(DateTimeOffset?))
                {
                    value = TypeDescriptor.GetConverter(Member.Type).ConvertFrom(value);
                }
                else
                {
                    value = Member.Type.IsEnum
                        ? System.Enum.Parse(Member.Type.IsNullable().underlyingType, value.ToString(), true)
                        : Convert.ChangeType(value, Member.Type.IsNullable().underlyingType, null);
                }
            }
            accessor[instanceOfObject, Member.Name] = value;
        }


 
    }


   

}
