using System;
using System.Reflection;
using FastMember;

namespace DotNetHelper.FastMember.Extension.Extension
{
    /// <summary>
    /// REFERENCE FROM https://stackoverflow.com/questions/21976125/how-to-get-the-attribute-data-of-a-member-with-fastmember
    /// </summary>
    public static class FastMemberExtension
    {

        public static T GetPrivateField<T>(this object obj, string name)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var type = obj.GetType();
            var field = type.GetField(name, flags);
            if (field == null) throw new NullReferenceException($"Failed to get private field {name} from object {obj.GetType().FullName}");
            return (T)field.GetValue(obj);
        }

        public static T GetPrivateProperty<T>(this object obj, string name)
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var type = obj.GetType();
            var field = type.GetProperty(name, flags);
            if (field == null) throw new NullReferenceException($"Failed to get private field {name} from object {obj.GetType().FullName}");
            return (T)field.GetValue(obj, null);
        }

        public static MemberInfo GetMemberInfo(this Member member)
        {
            return GetPrivateField<MemberInfo>(member, "member");
        }

        public static T GetMemberAttribute<T>(this Member member, bool inherit = false) where T : Attribute
        {
            return GetPrivateField<MemberInfo>(member, "member").GetCustomAttribute<T>(inherit);
        }

    }
}
