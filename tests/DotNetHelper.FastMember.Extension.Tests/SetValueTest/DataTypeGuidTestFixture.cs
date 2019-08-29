using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DotNetHelper.FastMember.Extension.Tests.Extension;
using FastMember;
using NUnit.Framework;

namespace DotNetHelper.FastMember.Extension.Tests.SetValueTest
{
    [TestFixture]
    public  class DataTypeGuidTestFixture
    {

        private class GuidDataType
        {
            public Guid GuidValue { get; set; } = Guid.Parse("63559BC0-1FEF-4158-968E-AE4B94974F8E");
            public Guid? NullValue { get; set; } = null;
          
        }

        public static TypeAccessor MyAccessor { get; } = TypeAccessor.Create(typeof(GuidDataType));
        private static GuidDataType Instance { get; } = new GuidDataType();
        private static DataTable GetData()
        {
            var table = new DataTable();



            MyAccessor.GetMembers().ToList().ForEach(delegate (Member member)
            {
                table.Columns.Add(member.Name, IsNullable(member.Type));
            });

            var values = new List<object>() { };
            MyAccessor.GetMembers().ToList().ForEach(delegate (Member member)
            {
                values.Add(MyAccessor[Instance, member.Name]);
            });
            table.Rows.Add(values.ToArray());
            table.AcceptChanges();
            return table;
        }



        [Test]
        public void Test_Map_Object_DataType_To_Strongly_Type_Class()
        {
            var dataReader = GetData().CreateDataReader();

            var list = dataReader.MapToList<GuidDataType>();
            var instance = list.First();


         ;

         MyAccessor.GetMembers().ToList().ForEach(delegate (Member member)
         {
             var expectedValue = MyAccessor[Instance, member.Name];
             var actualValue = MyAccessor[instance, member.Name];
                Assert.AreEqual(expectedValue,actualValue,
                    $"MapToList gave the property {member.Name} The wrong value. Expected {expectedValue} but it was {actualValue}");
            });




        }


        /// <summary>
        /// returns a tuple with the first value indicating if the specified type is a Nullable type and the second value indicating the underlyingtype
        /// if type is not nullable the underlyingType with be the specified type provided
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type IsNullable(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var childType = Nullable.GetUnderlyingType(type);
                return  childType;
            }
            return type;
        }

    }
}
