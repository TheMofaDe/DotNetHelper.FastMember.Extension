﻿using System;
using System.ComponentModel;
using System.Reflection;
using DotNetHelper.FastMember.Extension.Extension;
using FastMember;

namespace DotNetHelper.FastMember.Extension.Models
{
    public class AdvanceMember
    {
        public Member Member { get; }
        
        public AdvanceMember(Member member)
        {
            Member = member;
        }

        public T GetCustomAttribute<T>() where T : Attribute
        {
            if (Member.IsDefined(typeof(T)))
                return Member.GetMemberAttribute<T>(); // TODO :: CHECK TO SEE IF THIS RETURN NULL IF NOT DEFINED IF SO REMOVE THE CHECK ALSO WRITE UNIT TO ENSURE WE ARE NOTIFIED WHEN THIS FUNCTIONALITY CHANGES
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
                accessor[instanceOfObject, Member.Name] = null;
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
