using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetHelper.FastMember.Extension.Interface
{
    public interface IMember
    {
       string Name { get; }
       Type Type { get; }
       bool CanRead { get; }
       bool CanWrite { get; }
       T GetCustomAttribute<T>() where T : Attribute;
    }
}
