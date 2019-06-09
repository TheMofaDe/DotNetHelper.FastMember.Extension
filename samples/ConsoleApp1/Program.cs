using System;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using DotNetHelper.FastMember.Extension;
using FastMember;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var employee = new Employee()
            {
                DOB = DateTime.Now
                ,Name =  "Joe"
            };
          

            dynamic joe = new ExpandoObject();
            joe.Home = "No";
            joe.Day = true;


            var t = joe.GetType();
        
            var accessor =  TypeAccessor.Create(joe.GetType(), true);
            var members = accessor.GetMembers().ToList();

            

          //  var members3 = ExtFastMember.GetDynamicAdvanceMembers(joe);
            //members.ForEach(delegate(Member member)
            //{
            //    var attr = member.GetAttribute(typeof(RequiredAttribute), true);
            //    var attrNotInherit = member.GetAttribute(typeof(RequiredAttribute), false);
            //    var a = member.IsDefined(typeof(RequiredAttribute));
            //    var b = member.IsDefined(typeof(KeyAttribute));
            //    var c = GetMemberAttribute<RequiredAttribute>(member);
            //    var key = member.GetAttribute(typeof(KeyAttribute), true);
            //    var keyNotInherit = member.GetAttribute(typeof(KeyAttribute), false);
            //});

            Console.ReadLine();
        }


        public static void DynamicsOnly<T>(T obj) where T : IDynamicMetaObjectProvider
        {

        } 

        public static T GetPrivateField<T>( object obj, string name)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = obj.GetType();
            FieldInfo field = type.GetField(name, flags);
            return (T)field.GetValue(obj);
        }

        public static T GetPrivateProperty<T>( object obj, string name)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = obj.GetType();
            PropertyInfo field = type.GetProperty(name, flags);
            return (T)field.GetValue(obj, null);
        }

        public static MemberInfo GetMemberInfo( Member member)
        {
            return GetPrivateField<MemberInfo>(member, "member");
        }

        public static T GetMemberAttribute<T>( Member member) where T : Attribute
        {
            return GetPrivateField<MemberInfo>(member, "member").GetCustomAttribute<T>();
        }

    }



    public class Employee
    {
        [Required]
        public string Name { get; set; }
        [Key]
        public DateTime? DOB { get; set; }
    }
}
