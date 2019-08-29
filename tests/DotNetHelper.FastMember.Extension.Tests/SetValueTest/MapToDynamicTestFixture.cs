using System;
using System.Data;
using System.Dynamic;
using System.Linq;
using DotNetHelper.FastMember.Extension.Tests.Extension;
using NUnit.Framework;

namespace DotNetHelper.FastMember.Extension.Tests.SetValueTest
{
    [TestFixture]
    public class MapToDynamicTestFixture
    {

        //private class DynamicDataType
        //{
        //    public dynamic NumberValue { get; set; } = 1;
        //    public dynamic FloatValue { get; set; } = float.Parse("1");
        //    public dynamic LongValue { get; set; } = long.Parse("1");
        //    public dynamic NullValue { get; set; } = null;
        //    public dynamic StringValue { get; set; } = "TEST";
        //    public dynamic DecimalValue { get; set; } = 2.5;
        //    public dynamic DateTimeValue { get; set; } = DateTime.Today;
        //    public dynamic CharValue { get; set; } = 'A';
        //    public dynamic GuidValue { get; set; } = Guid.Parse("63559BC0-1FEF-4158-968E-AE4B94974F8E");
        //    public dynamic TimeSpanValue { get; set; } = TimeSpan.FromSeconds(10);
        //}

        private static DataTable GetData()
        {
            var table = new DataTable();

            table.Columns.Add("NumberValue", typeof(int));
            table.Columns.Add("FloatValue", typeof(float));
            table.Columns.Add("LongValue", typeof(long));
            table.Columns.Add("NullValue", typeof(object));
            table.Columns.Add("StringValue", typeof(string));
            table.Columns.Add("DecimalValue", typeof(decimal));
            table.Columns.Add("DateTimeValue", typeof(DateTime));
            table.Columns.Add("CharValue", typeof(char));
            table.Columns.Add("GuidValue", typeof(Guid));
            table.Columns.Add("TimeSpanValue", typeof(TimeSpan));

            table.Rows.Add(new object[] {1,float.Parse("1"),long.Parse("1"),DBNull.Value,"TEST",2.5,DateTime.Today,'A', Guid.Parse("63559BC0-1FEF-4158-968E-AE4B94974F8E"), TimeSpan.FromSeconds(10) });
            table.AcceptChanges();
            return table;
        }



        [Test]
        public void Test_Map_Dynamic_DataType_To_Strongly_Type_Class()
        {
            var dataReader = GetData().CreateDataReader();

            var list = dataReader.MapToList<dynamic>();
            Assert.IsNotNull(list,"Failed to create a list");
            Assert.IsNotEmpty(list, "Failed to put data in list");
            var instance = list.First();
            Assert.IsNotNull(instance, "Failed to create dynamic object");

            Assert.AreEqual(instance.NumberValue,1);
            Assert.AreEqual(instance.FloatValue, 1);
            Assert.AreEqual(instance.LongValue, 1);
            Assert.AreEqual(instance.NullValue, null);
            Assert.AreEqual(instance.DateTimeValue, DateTime.Today);
            Assert.AreEqual(instance.GuidValue, Guid.Parse("63559BC0-1FEF-4158-968E-AE4B94974F8E"));
            Assert.AreEqual(instance.TimeSpanValue, TimeSpan.FromSeconds(10));
            Assert.AreEqual(instance.StringValue, "TEST");
        }


        [Test]
        public void Test_Map_Dynamic_DataType_To_ExpandoObject()
        {
            var dataReader = GetData().CreateDataReader();

            var list = dataReader.MapToList<ExpandoObject>();
            Assert.IsNotNull(list, "Failed to create a list");
            Assert.IsNotEmpty(list, "Failed to put data in list");
            var instance = list.First() as dynamic;
            Assert.IsNotNull(instance, "Failed to create dynamic object");

            Assert.AreEqual(instance.NumberValue, 1);
            Assert.AreEqual(instance.FloatValue, 1);
            Assert.AreEqual(instance.LongValue, 1);
            Assert.AreEqual(instance.NullValue, null);
            Assert.AreEqual(instance.DateTimeValue, DateTime.Today);
            Assert.AreEqual(instance.GuidValue, Guid.Parse("63559BC0-1FEF-4158-968E-AE4B94974F8E"));
            Assert.AreEqual(instance.TimeSpanValue, TimeSpan.FromSeconds(10));
            Assert.AreEqual(instance.StringValue, "TEST");
        }


        [Test]
        public void Test_Map_Dynamic_DataType_To_Object()
        {
            var dataReader = GetData().CreateDataReader();

            var list = dataReader.MapToList<object>();
            Assert.IsNotNull(list, "Failed to create a list");
            Assert.IsNotEmpty(list, "Failed to put data in list");
            var instance = list.First() as dynamic;
            Assert.IsNotNull(instance, "Failed to create dynamic object");

            Assert.AreEqual(instance.NumberValue, 1);
            Assert.AreEqual(instance.FloatValue, 1);
            Assert.AreEqual(instance.LongValue, 1);
            Assert.AreEqual(instance.NullValue, null);
            Assert.AreEqual(instance.DateTimeValue, DateTime.Today);
            Assert.AreEqual(instance.GuidValue, Guid.Parse("63559BC0-1FEF-4158-968E-AE4B94974F8E"));
            Assert.AreEqual(instance.TimeSpanValue, TimeSpan.FromSeconds(10));
            Assert.AreEqual(instance.StringValue, "TEST");
        }




    }
}
