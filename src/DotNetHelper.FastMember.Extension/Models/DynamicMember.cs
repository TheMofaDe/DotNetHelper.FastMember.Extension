using System;
using System.ComponentModel;
using System.Dynamic;
using DotNetHelper.FastMember.Extension.Extension;
using DotNetHelper.FastMember.Extension.Interface;
using FastMember;

namespace DotNetHelper.FastMember.Extension.Models
{
    public class DynamicMember : IMember
    {
        public string Name { get; }
        public Type Type { get; }
        public bool CanRead { get; } = true;
        public bool CanWrite { get; } = true;

        public DynamicMember(string name, Type type)
        {
            Name = name;
            Type = type;

        }


        public T GetCustomAttribute<T>() where T : Attribute
        {
            return null; // dynamics object are created at runtime therefore there is no attributes available 
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
