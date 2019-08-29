using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using DotNetHelper.FastMember.Extension.Tests.Extension;
using FastMember;
using NUnit.Framework;

namespace DotNetHelper.FastMember.Extension.Tests.SetValueTest
{
    [TestFixture]
    public class DataTypeDynamicTestFixture
    {

        private class DynamicDataType
        {
            public dynamic NumberValue { get; set; } = 1;
            public dynamic FloatValue { get; set; } = float.Parse("1");
            public dynamic LongValue { get; set; } = long.Parse("1");
            public dynamic NullValue { get; set; } = null;
            public dynamic StringValue { get; set; } = "TEST";
            public dynamic DecimalValue { get; set; } = 2.5;
            public dynamic DateTimeValue { get; set; } = DateTime.Today;
            public dynamic CharValue { get; set; } = 'A';
            // DOESN"T KNOW HOW TO MAP A GUID TO A TYPE Dynamic
            public dynamic GuidValue { get; set; } = Guid.Parse("63559BC0-1FEF-4158-968E-AE4B94974F8E");
            // DOESN"T KNOW HOW TO MAP A TimeSpan TO A TYPE Dynamic
            public dynamic TimeSpanValue { get; set; } = TimeSpan.FromSeconds(10);
        }

        public static TypeAccessor MyAccessor { get; } = TypeAccessor.Create(typeof(DynamicDataType));
        private static DynamicDataType Instance { get; } = new DynamicDataType();
        private static DataTable GetData()
        {
            var table = new DataTable();


            MyAccessor.GetMembers().ToList().ForEach(delegate (Member member)
            {
                var test = IsTypeDynamic(member.Type);
                table.Columns.Add(member.Name, member.Type);
            });

            var values = new List<dynamic>() { };
            MyAccessor.GetMembers().ToList().ForEach(delegate (Member member)
            {
                values.Add(MyAccessor[Instance, member.Name]);
            });
            table.Rows.Add(values.ToArray());
            table.AcceptChanges();
            return table;
        }



        [Test]
        public void Test_Map_Dynamic_DataType_To_Strongly_Type_Class()
        {
            var dataReader = GetData().CreateDataReader();

            var list = dataReader.MapToList<DynamicDataType>();
            var instance = list.First();




            MyAccessor.GetMembers().ToList().ForEach(delegate (Member member)
            {
                var expectedValue = MyAccessor[Instance, member.Name];
                var actualValue = MyAccessor[instance, member.Name];
                Assert.AreEqual(expectedValue, actualValue,
                    $"MapToList gave the property {member.Name} The wrong value. Expected {expectedValue} but it was {actualValue}");
            });




        }



        /// <summary>
        /// return true if the type is assignable form IDynamicMetaObjectProvider
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsTypeDynamic(Type type)
        {
            return typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type);
        }

    }
}
