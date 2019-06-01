using System;
using System.Collections.Generic;
using System.Text;
using FastMember;

namespace DotNetHelper.FastMember.Extension.Models
{
    public class Property
    {
        public string Name { get; }
        public Type Type { get; }
        public bool CanRead { get; }
        public bool CanWrite { get; }
        internal Property(Member member)
        {
            Name = member.Name;
            Type = member.Type;
            CanRead = member.CanRead;
            CanWrite = member.CanWrite;
        }

        internal Property(string name, object value)
        {
            Name = name;
            Type = value == null ? typeof(object) : value.GetType();
            CanWrite = true;
            CanRead = true;
        }


        public T GetCustomAttribute<T>() where T : Attribute
        {
            return null;
        }

    }
}
