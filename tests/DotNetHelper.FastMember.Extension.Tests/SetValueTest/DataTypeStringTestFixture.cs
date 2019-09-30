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
    public class DataTypeStringTestFixture
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
            public object GuidValue { get; set; } = Guid.Parse("63559BC0-1FEF-4158-968E-AE4B94974F8E");
            public object TimeSpanValue { get; set; } = TimeSpan.FromSeconds(10);
        }

        private class StringDataType
        {
            public string NumberValue { get; set; } = 1.ToString();
            public string FloatValue { get; set; } = float.Parse("1").ToString();
            public string LongValue { get; set; } = long.Parse("1").ToString();
            public string NullValue { get; set; } = null;
            public string StringValue { get; set; } = "TEST";
            public string DecimalValue { get; set; } = 2.5.ToString();
            public string DateTimeValue { get; set; } = DateTime.Today.ToString();
            public string CharValue { get; set; } = "A";
            public string GuidValue { get; set; } = Guid.Parse("63559BC0-1FEF-4158-968E-AE4B94974F8E").ToString();
            public string TimeSpanValue { get; set; } = TimeSpan.FromSeconds(10).ToString();
        }

        public static TypeAccessor ObjectDataTypeAccessor { get; } = TypeAccessor.Create(typeof(ObjectDataType));
        public static TypeAccessor StringAccessor { get; } = TypeAccessor.Create(typeof(StringDataType));
        private static ObjectDataType Instance { get; } = new ObjectDataType();
        private static DataTable GetData()
        {
            var table = new DataTable();

            ObjectDataTypeAccessor.GetMembers().ToList().ForEach(delegate (Member member)
            {
                table.Columns.Add(member.Name, member.Type);
            });

            var values = new List<object>() { };
            ObjectDataTypeAccessor.GetMembers().ToList().ForEach(delegate (Member member)
            {
                values.Add(ObjectDataTypeAccessor[Instance, member.Name]);
            });
            table.Rows.Add(values.ToArray());
            table.AcceptChanges();
            return table;
        }



        [Test]
        public void Test_Map_Object_DataType_To_Strongly_Type_Class()
        {
            var dataReader = GetData().CreateDataReader();

            var list = dataReader.MapToList<StringDataType>();
            var instance = list.First();

            ObjectDataTypeAccessor.GetMembers().ToList().ForEach(delegate (Member member)
            {
                var expectedValue = ObjectDataTypeAccessor[Instance, member.Name];
                var actualValue = StringAccessor[instance, member.Name];
                Assert.AreEqual(expectedValue?.ToString(), actualValue, $"MapToList gave the property {member.Name} The wrong value. Expected {expectedValue} but it was {actualValue}");
            });

        }

    }
}
