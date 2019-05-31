using System;
using System.ComponentModel;
using System.Dynamic;
using DotNetHelper.FastMember.Extension.Extension;
using FastMember;

namespace DotNetHelper.FastMember.Extension.Models
{
    public class DynamicAdvanceMember
    {
        public string Name { get; }
        public Type Type { get; }
      //  public object Value { get; }


        public DynamicAdvanceMember(string name, Type type)
        {
            Name = name;
            Type = type;
           // Value = value;
        }

        public void SetMemberValue<T>(T instanceOfObject, object value) where T : IDynamicMetaObjectProvider
        {
            instanceOfObject.IsNullThrow(nameof(instanceOfObject)); 
            var accessor = TypeAccessor.Create(typeof(T), true);


            if (value == null)
            {
                accessor[instanceOfObject, Name] = null; // TODO :: WRITE MANY UNIT TEST TO TRY & BREAK THIS

                return;
            }
            if (value.GetType() != Type) // TODO :: UNIT TEST FOR EVERY SINGLE SYSTEM TYPE
            {
                if (Type == typeof(DateTimeOffset) || Type == typeof(DateTimeOffset?))
                {
                    value = TypeDescriptor.GetConverter(Type).ConvertFrom(value);
                }
                else
                {
                    value = Type.IsEnum
                        ? System.Enum.Parse(Type.IsNullable().underlyingType, value.ToString(), true)
                        : Convert.ChangeType(value, Type.IsNullable().underlyingType, null);
                }
            }
            accessor[instanceOfObject, Name] = value;
        }

        public object GetValue<T>(T instanceOfObject) where T : IDynamicMetaObjectProvider
        {
            var accessor = TypeAccessor.Create(typeof(T), true);
            return accessor[instanceOfObject, Name];
        }

    }
}
