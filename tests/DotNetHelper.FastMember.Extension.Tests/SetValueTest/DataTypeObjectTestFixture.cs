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
    public  class DataTypeObjectTestFixture
    {

        private class ObjectDataType
        {
            public object NumberValue { get; set; } = 1;
            public object FloatValue { get; set; } = float.Parse("1");
            public object LongValue { get; set; } = long.Parse("1");
            public object NullValue { get; set; } = null;
            public object StringValue { get; set; } = "TEST";
            public object DecimalValue { get; set; } = 2.5;
            public object DateTimeValue { get; set; } = DateTime.Today;
            public object CharValue { get; set; } = 'A';
            // DOESN"T KNOW HOW TO MAP A GUID TO A TYPE OBJECT
            public object GuidValue { get; set; } = Guid.Parse("63559BC0-1FEF-4158-968E-AE4B94974F8E");

           // DOESN"T KNOW HOW TO MAP A TimeSpan TO A TYPE OBJECT
            public object TimeSpanValue { get; set; } = TimeSpan.FromSeconds(10);
        }

        public static TypeAccessor MyAccessor { get; } = TypeAccessor.Create(typeof(ObjectDataType));
        private static ObjectDataType Instance { get; } = new ObjectDataType();
        private static DataTable GetData()
        {
            var table = new DataTable();

            MyAccessor.GetMembers().ToList().ForEach(delegate (Member member)
            {
                table.Columns.Add(member.Name, member.Type);
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

            var list = dataReader.MapToList<ObjectDataType>();
            var instance = list.First();


         

         MyAccessor.GetMembers().ToList().ForEach(delegate (Member member)
         {
             var expectedValue = MyAccessor[Instance, member.Name];
             var actualValue = MyAccessor[instance, member.Name];
                Assert.AreEqual(expectedValue,actualValue,
                    $"MapToList gave the property {member.Name} The wrong value. Expected {expectedValue} but it was {actualValue}");
            });




        }

    }
}
