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

            // CREATE A GENERIC, DYNAMIC, & ANONYMOUS OBJECT 
            var employee = new Employee() { FirstName = "generic" };
            dynamic dynamicEmployee = new ExpandoObject();
            var anonymousEmployee = new { FirstName = "I'm so Anonymous" };

            // SET PROPERTY VALUE FOR GENERICS & DYNAMICS OBJECTS
            ExtFastMember.SetMemberValue(employee, "FirstName", "I'm so generic");
            ExtFastMember.SetMemberValue(dynamicEmployee, "FirstName", "I'm so Dynamic");

            // GET PROPERTY VALUES FOR GENERICS & DYNAMICS & ANONYMOUS OBJECTS
            Console.WriteLine(ExtFastMember.GetMemberValue(employee,"FirstName")); // PRINTS : I'm so generic
            Console.WriteLine(ExtFastMember.GetMemberValue(dynamicEmployee, "FirstName"));  // PRINTS : I'm so Dynamic
            Console.WriteLine(ExtFastMember.GetMemberValue(anonymousEmployee, "FirstName"));  // PRINTS : I'm so Anonymous

        }




    }



    public class Employee
    {
        [Required]
        public string FirstName { get; set; }
        [Key]
        public DateTime? DOB { get; set; }
    }
}
